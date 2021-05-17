using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;
using RaidExtractor.Core.Native;

namespace RaidExtractor.Core
{
    public class Extractor
    {
        private readonly Dictionary<int, HeroType> heroTypeById;
        private readonly StatMultiplier[] multipliers;

        public Extractor()
        {
            heroTypeById =  Deserialize<HeroType[]>("RaidExtractor.Core.hero_types.json").ToDictionary(t => t.Id);
            multipliers = Deserialize<StatMultiplier[]>("RaidExtractor.Core.multipliers.json");
        }

        private static T Deserialize<T>(string resourceName)
        {
            var serializer = new JsonSerializer();
            var assembly = typeof(Extractor).Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var sr = new StreamReader(stream))
            using (var textReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(textReader);
            }
        }

        public AccountDump GetDump()
        {
            var process = IsRaidRunning();
            if (process == null)
            {
                return null;
            }

            if (!CheckRaidVersion(process))
            {
                return null;
            }

            var handle = NativeWrapper.OpenProcess(ProcessAccessFlags.Read, true, process.Id);
            try
            {
                var gameAssembly = GetRaidAssembly(process);

                var klass = IntPtr.Zero;
                NativeWrapper.ReadProcessMemory(handle, gameAssembly.BaseAddress + (int)StaticDataHandler.Instance.GetValue("MemoryLocation"), ref klass);

                var appModel = klass;
                // These Reposition the AppModel to be in the right place. TODO: Figure out where these magic numbers come from?
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x18, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0xC0, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x0, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0xB8, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x8, ref appModel);

                var userWrapper = appModel;
                NativeWrapper.ReadProcessMemory(handle, userWrapper + (int)StaticDataHandler.Instance.GetValue("AppModelUserWrapper"), ref userWrapper); // AppModel._userWrapper

                var heroesWrapper = userWrapper;
                NativeWrapper.ReadProcessMemory(handle, heroesWrapper + (int)StaticDataHandler.Instance.GetValue("UserWrapperHeroes"), ref heroesWrapper); // UserWrapper.Heroes

#region Artifact Extraction

                var artifactsPointer = heroesWrapper;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + (int)StaticDataHandler.Instance.GetValue("HeroesWrapperArtifactData"), ref artifactsPointer); // HeroesWrapperReadOnly.ArtifactData
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + (int)StaticDataHandler.Instance.GetValue("UserArtifactDataArtifacts"), ref artifactsPointer); // UserArtifactData.Artifacts

                var artifactCount = 0;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + RaidStaticInformation.ListCount, ref artifactCount); // List<Artifact>.Count

                var pointers = new List<IntPtr>();
                if (artifactCount > 0)
                {
                    var arrayPointer = artifactsPointer;
                    NativeWrapper.ReadProcessMemory(handle, artifactsPointer + RaidStaticInformation.ListIndexArray, ref arrayPointer); // List<Artifact>._array

                    var ptrs = new IntPtr[artifactCount];
                    NativeWrapper.ReadProcessMemoryArray(handle, arrayPointer + RaidStaticInformation.ListElementPointerArray, ptrs);
                    pointers.AddRange(ptrs);

                }

                if (artifactCount == 0)
                {
                    // This means it's in external storage instead which is in a concurrent dictionary (teh sucks)
                    NativeWrapper.ReadProcessMemory(handle, gameAssembly.BaseAddress + (int)StaticDataHandler.Instance.GetValue("ExternalStorageAddress"), ref klass);

                    var artifactStorageResolver = klass;
                    NativeWrapper.ReadProcessMemory(handle, artifactStorageResolver + 0xB8, ref artifactStorageResolver); // ArtifactStorageResolver-StaticFields
                    NativeWrapper.ReadProcessMemory(handle, artifactStorageResolver, ref artifactStorageResolver); // ArtifactStorageResolver-StaticFields._implementation

                    var state = artifactStorageResolver;
                    NativeWrapper.ReadProcessMemory(handle, state + 0x10, ref state); // ExternalArtifactsStorage._state

                    artifactsPointer = state;
                    NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x18, ref artifactsPointer); // _state._artifacts

                    var buckets = artifactsPointer;
                    NativeWrapper.ReadProcessMemory(handle, buckets + 0x10, ref buckets); // ConcurrentDictionary._tables
                    NativeWrapper.ReadProcessMemory(handle, buckets + 0x10, ref buckets); // _tables._buckets

                    var bucketCount = 0;
                    NativeWrapper.ReadProcessMemory(handle, buckets + 0x18, ref bucketCount);

                    var nodes = new IntPtr[bucketCount];
                    if (bucketCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, buckets + RaidStaticInformation.ListElementPointerArray, nodes);

                    for (var i = 0; i < nodes.Length; i++)
                    {
                        var node = nodes[i];
                        while (node != IntPtr.Zero)
                        {
                            var pointer = node;
                            NativeWrapper.ReadProcessMemory(handle, pointer + 0x18, ref pointer); // Node.m_value
                            if (pointer != IntPtr.Zero) pointers.Add(pointer);
                            NativeWrapper.ReadProcessMemory(handle, node + 0x20, ref node); // Node.m_next
                        }
                    }
                }

                var artifacts = new List<Artifact>();
                var artifactStruct = new ArtifactStruct();
                var artifactBonusStruct = new ArtifactBonusStruct();
                var bonusValueStruct = new BonusValueStruct();
                foreach (var pointer in pointers)
                {
                    NativeWrapper.ReadProcessMemory(handle, pointer, ref artifactStruct);
                    NativeWrapper.ReadProcessMemory(handle, artifactStruct.PrimaryBonus, ref artifactBonusStruct);
                    NativeWrapper.ReadProcessMemory(handle, artifactBonusStruct.Value, ref bonusValueStruct);

                    var artifact = new Artifact
                    {
                        Id = artifactStruct.Id,
                        SellPrice = artifactStruct.SellPrice,
                        Price = artifactStruct.Price,
                        Level = artifactStruct.Level,
                        IsActivated = artifactStruct.IsActivated,
                        Kind = artifactStruct.KindId.ToString(),
                        Rank = artifactStruct.RankId.ToString(),
                        Rarity = artifactStruct.RarityId.ToString(),
                        SetKind = artifactStruct.SetKindId.ToString(),
                        IsSeen = artifactStruct.IsSeen,
                        FailedUpgrades = artifactStruct.FailedUpgrades
                    };

                    if (artifactStruct.RequiredFraction != HeroFraction.Unknown)
                        artifact.RequiredFraction = artifactStruct.RequiredFraction.ToString();

                    artifact.PrimaryBonus = new ArtifactBonus
                    {
                        Kind = artifactBonusStruct.KindId.ToString(),
                        IsAbsolute = bonusValueStruct.IsAbsolute,
                        Value = (float)Math.Round(bonusValueStruct.Value / (double)uint.MaxValue, 2),
                    };

                    var bonusesPointer = artifactStruct.SecondaryBonuses;
                    var bonusCount = 0;
                    NativeWrapper.ReadProcessMemory(handle, bonusesPointer + RaidStaticInformation.ListCount, ref bonusCount);
                    NativeWrapper.ReadProcessMemory(handle, bonusesPointer + RaidStaticInformation.ListIndexArray, ref bonusesPointer);

                    artifact.SecondaryBonuses = new List<ArtifactBonus>();

                    var bonuses = new IntPtr[bonusCount];
                    if (bonusCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, bonusesPointer + RaidStaticInformation.ListElementPointerArray, bonuses, 0, bonuses.Length);

                    foreach (var bonusPointer in bonuses)
                    {
                        NativeWrapper.ReadProcessMemory(handle, bonusPointer, ref artifactBonusStruct);
                        NativeWrapper.ReadProcessMemory(handle, artifactBonusStruct.Value, ref bonusValueStruct);

                        var bonus = new ArtifactBonus
                        {
                            Kind = artifactBonusStruct.KindId.ToString(),
                            IsAbsolute = bonusValueStruct.IsAbsolute,
                            Value = (float)Math.Round(bonusValueStruct.Value / (double)uint.MaxValue, 2),
                            Enhancement = (float)Math.Round(artifactBonusStruct.PowerUpValue / (double)uint.MaxValue, 2),
                            Level = artifactBonusStruct.Level
                        };

                        artifact.SecondaryBonuses.Add(bonus);
                    }

                    artifacts.Add(artifact);
                }

                artifacts = artifacts.OrderBy(o => o.Id).ToList();
#endregion

#region Hero Extraction

                var heroesDataPointer = heroesWrapper;
                NativeWrapper.ReadProcessMemory(handle, heroesDataPointer + (int)StaticDataHandler.Instance.GetValue("HeroesWrapperHeroData"), ref heroesDataPointer); // HeroesWrapperReadOnly.HeroData
                NativeWrapper.ReadProcessMemory(handle, heroesDataPointer + (int)StaticDataHandler.Instance.GetValue("UserHeroDataHeroById"), ref heroesDataPointer); // UserHeroData.HeroById

                var count = 0;
                NativeWrapper.ReadProcessMemory(handle, heroesDataPointer + RaidStaticInformation.DictionaryCount, ref count); // Dictionary<int, Hero>.Count
                NativeWrapper.ReadProcessMemory(handle, heroesDataPointer + RaidStaticInformation.DictionaryEntries, ref heroesDataPointer); // Dictionary<int, Hero>.entries

                var heroStruct = new HeroStruct();
                var heroMasteriesStruct = new HeroMasteryDataStruct();
                var skillStruct = new SkillStruct();
                var heroesById = new Dictionary<int, Hero>();
                var heroes = new List<Hero>();
                for (var i = 0; i < count; i++)
                {
                    // Array of Dictionary-entry structs which are 0x18 in size (but we only need hero pointer)
                    var heroPointer = heroesDataPointer + 0x30 + 0x18 * i;
                    NativeWrapper.ReadProcessMemory(handle, heroPointer, ref heroPointer);
                    NativeWrapper.ReadProcessMemory(handle, heroPointer, ref heroStruct);

                    heroMasteriesStruct.Masteries = IntPtr.Zero;
                    NativeWrapper.ReadProcessMemory(handle, heroStruct.MasteryData, ref heroMasteriesStruct);

                    var hero = new Hero
                    {
                        Id = heroStruct.Id,
                        TypeId = heroStruct.TypeId,
                        Grade = heroStruct.Grade.ToString(),
                        Level = heroStruct.Level,
                        Experience = heroStruct.Experience,
                        FullExperience = heroStruct.FullExperience,
                        Locked = heroStruct.Locked,
                        InStorage = heroStruct.InStorage,
                        Marker = heroStruct.Marker.ToString(),
                        Masteries = new List<int>(),
                    };

                    var masteriesPtr = heroMasteriesStruct.Masteries;
                    var masteryCount = 0;
                    if (heroStruct.MasteryData != IntPtr.Zero && masteriesPtr != IntPtr.Zero)
                    {
                        NativeWrapper.ReadProcessMemory(handle, masteriesPtr + RaidStaticInformation.ListCount, ref masteryCount);
                        NativeWrapper.ReadProcessMemory(handle, masteriesPtr + RaidStaticInformation.ListIndexArray, ref masteriesPtr);
                    }

                    var masteries = new int[masteryCount];
                    if (masteryCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, masteriesPtr + RaidStaticInformation.ListElementPointerArray, masteries, 0, masteries.Length);
                    hero.Masteries.AddRange(masteries);

                    var skillsPtr = heroStruct.Skills;
                    var skillsCount = 0;
                    if (skillsPtr != IntPtr.Zero)
                    {
                        NativeWrapper.ReadProcessMemory(handle, heroStruct.Skills + RaidStaticInformation.ListCount, ref skillsCount);
                        NativeWrapper.ReadProcessMemory(handle, heroStruct.Skills + RaidStaticInformation.ListIndexArray, ref skillsPtr);
                    }

                    hero.Skills = new List<Skill>();
                    var skills = new IntPtr[skillsCount];
                    if (skillsCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, skillsPtr + RaidStaticInformation.ListElementPointerArray, skills, 0, skills.Length);

                    foreach (var skillPointer in skills)
                    {
                        NativeWrapper.ReadProcessMemory(handle, skillPointer, ref skillStruct);

                        var skill = new Skill
                        {
                            Id = skillStruct.Id,
                            TypeId = skillStruct.TypeId,
                            Level = skillStruct.Level,
                        };

                        hero.Skills.Add(skill);
                    }

                    if (heroTypeById.TryGetValue(hero.TypeId, out var heroType))
                    {
                        hero.Name = heroType.Name;
                        hero.Fraction = heroType.Fraction;
                        hero.Element = heroType.Element;
                        hero.Rarity = heroType.Rarity;
                        hero.Role = heroType.Role;
                        hero.AwakenLevel = heroType.AwakeLevel;
                        hero.Accuracy = heroType.Accuracy;
                        hero.Attack = heroType.Attack;
                        hero.Defense = heroType.Defense;
                        hero.Health = heroType.Health;
                        hero.Speed = heroType.Speed;
                        hero.Resistance = heroType.Resistance;
                        hero.CriticalChance = heroType.CriticalChance;
                        hero.CriticalDamage = heroType.CriticalDamage;
                        hero.CriticalHeal = heroType.CriticalHeal;
                    }

                    var multiplier = multipliers.FirstOrDefault(m => m.Stars == hero.Grade && m.Level == hero.Level);
                    if (multiplier != null)
                    {
                        hero.Attack = (int)Math.Round(hero.Attack * multiplier.Multiplier);
                        hero.Defense = (int)Math.Round(hero.Defense * multiplier.Multiplier);
                        hero.Health = (int)Math.Round(hero.Health * multiplier.Multiplier) * 15;
                    }

                    heroes.Add(hero);

                    heroesById[heroStruct.Id] = hero;
                }

                heroes = heroes.OrderBy(o => o.Id).ToList();
#endregion

#region Hero Artifact Extraction

                var artifactsByHeroIdPtr = heroesWrapper;
                NativeWrapper.ReadProcessMemory(handle, artifactsByHeroIdPtr + (int)StaticDataHandler.Instance.GetValue("HeroesWrapperArtifactData"), ref artifactsByHeroIdPtr); // HeroesWrapperReadOnly.ArtifactData
                NativeWrapper.ReadProcessMemory(handle, artifactsByHeroIdPtr + (int)StaticDataHandler.Instance.GetValue("UserArtifactArtifactDataByHeroId"), ref artifactsByHeroIdPtr); // UserArtifactData.ArtifactDataByHeroId

                NativeWrapper.ReadProcessMemory(handle, artifactsByHeroIdPtr + RaidStaticInformation.DictionaryCount, ref count);
                NativeWrapper.ReadProcessMemory(handle, artifactsByHeroIdPtr + 0x18, ref artifactsByHeroIdPtr);

                for (var i = 0; i < count; i++)
                {
                    artifactsPointer = artifactsByHeroIdPtr + 0x30 + 0x18 * i;

                    var heroId = 0;
                    NativeWrapper.ReadProcessMemory(handle, artifactsPointer - 8, ref heroId);
                    NativeWrapper.ReadProcessMemory(handle, artifactsPointer, ref artifactsPointer);
                    NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x10, ref artifactsPointer);

                    artifactCount = 0;
                    NativeWrapper.ReadProcessMemory(handle, artifactsPointer + RaidStaticInformation.DictionaryCount, ref artifactCount);
                    NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x18, ref artifactsPointer);

                    var arts = new List<int>();
                    for (var a = 0; a < artifactCount; a++)
                    {
                        var artifactId = 0;
                        NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x2C + 0x10 * a, ref artifactId);
                        arts.Add(artifactId);
                    }

                    if (heroesById.TryGetValue(heroId, out var hero)) hero.Artifacts = arts;
                }

#endregion

#region Arena League Extraction

                var arenaPtr = userWrapper;
                NativeWrapper.ReadProcessMemory(handle, userWrapper + (int)StaticDataHandler.Instance.GetValue("UserWrapperArena"), ref arenaPtr);

                ArenaLeagueId arenaLeague = 0;
                NativeWrapper.ReadProcessMemory(handle, arenaPtr + (int)StaticDataHandler.Instance.GetValue("ArenaWrapperLeagueId"), ref arenaLeague);

#endregion

#region Great Hall Extraction

                var villageDataPointer = IntPtr.Zero;
                NativeWrapper.ReadProcessMemory(handle, userWrapper + (int)StaticDataHandler.Instance.GetValue("UserWrapperCapitol"), ref villageDataPointer); // UserWrapper.Capitol
                NativeWrapper.ReadProcessMemory(handle, villageDataPointer + (int)StaticDataHandler.Instance.GetValue("CapitolWrapperVillageData"), ref villageDataPointer); // Capitol.VillageData

                var bonusLevelPointer = IntPtr.Zero;
                NativeWrapper.ReadProcessMemory(handle, villageDataPointer + (int)StaticDataHandler.Instance.GetValue("UserVillageDataCapitolBonusLevelByStatByElement"), ref bonusLevelPointer); // UserVillageData.UserVillageDataCapitolBonusLevelByStatByElement

                count = 0;
                NativeWrapper.ReadProcessMemory(handle, bonusLevelPointer + RaidStaticInformation.DictionaryCount, ref count); // Dictionary<Element, Dictionary<StatKindId, int>>.Count
                NativeWrapper.ReadProcessMemory(handle, bonusLevelPointer + RaidStaticInformation.DictionaryEntries, ref bonusLevelPointer); // Dictionary<Element, Dictionary<StatKindId, int>>.Entries

                var greatHall = new Dictionary<Element, Dictionary<StatKindId, int>>();

                for (var i = 0; i < count; i++)
                {
                    var elementPointer = bonusLevelPointer + 0x28 + 0x18 * i; // Dictionary<Element, Dictionary<StatKindId, int>>.Key;
                    var statDictionaryPointer = bonusLevelPointer + 0x30 + 0x18 * i; // Dictionary<Element, Dictionary<StatKindId, int>>.Value

                    Element currentElement = 0; // Initial value
                    NativeWrapper.ReadProcessMemory(handle, elementPointer, ref currentElement);
                    NativeWrapper.ReadProcessMemory(handle, statDictionaryPointer, ref statDictionaryPointer);

                    var statsCount = 0;
                    var statsPointer = IntPtr.Zero;
                    var statsDictionary = new Dictionary<StatKindId, int>();

                    NativeWrapper.ReadProcessMemory(handle, statDictionaryPointer + RaidStaticInformation.DictionaryCount, ref statsCount); // Dictionary<StatKindId, int>().Count
                    NativeWrapper.ReadProcessMemory(handle, statDictionaryPointer + RaidStaticInformation.DictionaryEntries, ref statsPointer); // Dictionary<StatKindId, int>().Entries
                    for (var j = 0; j < statsCount; j++)
                    {
                        var statKindPointer = statsPointer + 0x28 + 0x10 * j;
                        var statValuePointer = statsPointer + 0x2C + 0x10 * j;

                        StatKindId statKindId = 0;
                        int statLevel = 0;
                        NativeWrapper.ReadProcessMemory(handle, statKindPointer, ref statKindId);
                        NativeWrapper.ReadProcessMemory(handle, statValuePointer, ref statLevel);

                        statsDictionary.Add(statKindId, statLevel);
                    }
                    greatHall.Add(currentElement, statsDictionary);
                }

#endregion

#region Shard extraction
                var shards = new Dictionary<string, ShardInfo>();

                var shardsDataPointer = userWrapper;
                NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + (int)StaticDataHandler.Instance.GetValue("UserWrapperShards"), ref shardsDataPointer); // UserWrapper.ShardWrapperReadOnly
                NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + (int)StaticDataHandler.Instance.GetValue("ShardWrapperData"), ref shardsDataPointer); // ShardWrapperReadOnly.UserShardData

                var shardSummonDataPointer = shardsDataPointer;
                NativeWrapper.ReadProcessMemory(handle, shardSummonDataPointer + (int)StaticDataHandler.Instance.GetValue("ShardSummonData"), ref shardSummonDataPointer); // UserShardData.SummonResults

                // Read the shard count data
                NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + (int)StaticDataHandler.Instance.GetValue("ShardData"), ref shardsDataPointer); 

                var shardCount = 0;
                NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + RaidStaticInformation.ListCount, ref shardCount);

                List <IntPtr> shardPointers = new List<IntPtr>();
                if (shardCount > 0)
                {
                    var arrayPointer = shardsDataPointer;
                    NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + RaidStaticInformation.ListIndexArray, ref arrayPointer); 

                    var ptrs = new IntPtr[shardCount];
                    NativeWrapper.ReadProcessMemoryArray(handle, arrayPointer + RaidStaticInformation.ListElementPointerArray, ptrs);
                    shardPointers.AddRange(ptrs);
                }

                List<ShardStruct> shardCounts = new List<ShardStruct>();
                foreach (var pointer in shardPointers)
                {
                    ShardStruct countData = new ShardStruct();
                    NativeWrapper.ReadProcessMemory(handle, pointer, ref countData);

                    string key = countData.ShardTypeId.ToString();

                    if (!shards.ContainsKey(key))
                    {
                        shards.Add(key, new ShardInfo());
                        shards[key].SummonData = new List<ShardSummonInfo>();
                    }

                    shards[key].Count = countData.Count;
                }

                // Now read the summon data (pity system)
                var shardSummonCount = 0;
                NativeWrapper.ReadProcessMemory(handle, shardSummonDataPointer + RaidStaticInformation.ListCount, ref shardSummonCount);

                shardPointers = new List<IntPtr>();
                if (shardSummonCount > 0)
                {
                    var arrayPointer = shardSummonDataPointer;
                    NativeWrapper.ReadProcessMemory(handle, shardSummonDataPointer + RaidStaticInformation.ListIndexArray, ref arrayPointer);

                    var ptrs = new IntPtr[shardSummonCount];
                    NativeWrapper.ReadProcessMemoryArray(handle, arrayPointer + RaidStaticInformation.ListElementPointerArray, ptrs);
                    shardPointers.AddRange(ptrs);

                }

                List<ShardSummonData> shardSummons = new List<ShardSummonData>();
                foreach (var pointer in shardPointers)
                {
                    ShardSummonData summonData = new ShardSummonData();
                    NativeWrapper.ReadProcessMemory(handle, pointer, ref summonData);

                    string key = summonData.ShardTypeId.ToString();
                    if (!shards.ContainsKey(key))
                    {
                        // We might have to add an entry if the user has no shards of that type.
                        shards.Add(key, new ShardInfo());
                        shards[key].SummonData = new List<ShardSummonInfo>();
                        shards[key].Count = 0;
                    }

                    ShardSummonInfo info = new ShardSummonInfo();
                    info.Rarity = summonData.rarity.ToString();
                    info.LastHeroId = summonData.lastHeroId;
                    info.PullCount = summonData.pullCount;
                    shards[key].SummonData.Add(info);
                }

#endregion

#region BattlePresets extraction

                var presetsPointer = heroesWrapper;
                NativeWrapper.ReadProcessMemory(handle, presetsPointer + (int)StaticDataHandler.Instance.GetValue("HeroesWrapperHeroData"), ref presetsPointer); // HeroesWrapperReadOnly.HeroData
                NativeWrapper.ReadProcessMemory(handle, presetsPointer + (int)StaticDataHandler.Instance.GetValue("UserHeroDataBattlePresets"), ref presetsPointer); // UserHeroData.BattlePresets

                var battlePresetsCount = 0;
                var presetsDataPointer = IntPtr.Zero;

                NativeWrapper.ReadProcessMemory(handle, presetsPointer + RaidStaticInformation.DictionaryCount, ref battlePresetsCount); // Dictionary<int, int[]>.Count
                NativeWrapper.ReadProcessMemory(handle, presetsPointer + RaidStaticInformation.DictionaryEntries, ref presetsDataPointer); // Dictionary<int, int[]>.Entries

                var stagePresets = new Dictionary<int, int[]>();

                for (var i = 0; i < battlePresetsCount; i++)
                {
                    var stageIdPointer = presetsDataPointer + 0x28 + 0x18 * i; // Dictionary<int, int[]>.Key;
                    var heroIdArrayPointer = presetsDataPointer + 0x30 + 0x18 * i; // Dictionary<int, int[]>.Value
                    int currentStage = 0;

                    NativeWrapper.ReadProcessMemory(handle, stageIdPointer, ref currentStage);
                    NativeWrapper.ReadProcessMemory(handle, heroIdArrayPointer, ref heroIdArrayPointer);

                    var heroArrayCount = 0;
                    NativeWrapper.ReadProcessMemory(handle, heroIdArrayPointer + RaidStaticInformation.ListCount, ref heroArrayCount); // Assuming Array.Count has identical offset as List.Count

                    // Making use of ReadProcessMemoryArray instead of iterating the array ourselves
                    if (heroArrayCount > 0)
                    {
                        var heroIdArray = new int[heroArrayCount];
                        var heroIdPointer = heroIdArrayPointer + 0x20; // Location of the first entry

                        NativeWrapper.ReadProcessMemoryArray(handle, heroIdPointer, heroIdArray);
                        stagePresets[currentStage] = heroIdArray;
                    }
                }
				
#endregion

                return new AccountDump
                {
                    Artifacts = artifacts,
                    Heroes = heroes,
                    ArenaLeague = arenaLeague.ToString(),
                    GreatHall = greatHall,
                    Shards = shards,
                    StagePresets = stagePresets,  
                };
            }
            finally
            {
                NativeWrapper.CloseHandle(handle);
            }
        }

        private Process IsRaidRunning()
        {
            var process = Process.GetProcessesByName("Raid").FirstOrDefault();
            if (process == null)
            {
                throw new Exception("Raid needs to be running before running RaidExtractor");
            }

            return process;
        }

        private bool CheckRaidVersion(Process process)
        {
            if (!process.MainModule.FileName.Contains((string)StaticDataHandler.Instance.GetValue("ExpectedRaidVersion")))
            {
                StaticDataHandler.Instance.UpdateValuesFromGame(process.MainModule.FileName);
            }

            return true;
        }

        private ProcessModule GetRaidAssembly(Process process)
        {
            var gameAssembly = process.Modules.OfType<ProcessModule>().FirstOrDefault(m => m.ModuleName == "GameAssembly.dll");
            if (gameAssembly == null)
            {
                throw new Exception("Unable to locate GameAssembly.dll in memory");
            }

            return gameAssembly;
        }
    }
}
