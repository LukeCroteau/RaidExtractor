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
                NativeWrapper.ReadProcessMemory(handle, gameAssembly.BaseAddress + RaidStaticInformation.MemoryLocation, ref klass);

                var appModel = klass;
                // These Reposition the AppModel to be in the right place. TODO: Figure out where these magic numbers come from?
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x18, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0xC0, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x0, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0xB8, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x8, ref appModel);

                var userWrapper = appModel;
                NativeWrapper.ReadProcessMemory(handle, userWrapper + RaidStaticInformation.AppModelUserWrapper, ref userWrapper); // AppModel._userWrapper

                var heroesWrapper = userWrapper;
                NativeWrapper.ReadProcessMemory(handle, heroesWrapper + RaidStaticInformation.UserWrapperHeroes, ref heroesWrapper); // UserWrapper.Heroes

#region Artifact Extraction

                var artifactsPointer = heroesWrapper;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + RaidStaticInformation.HeroesWrapperArtifactData, ref artifactsPointer); // HeroesWrapperReadOnly.ArtifactData
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + RaidStaticInformation.UserArtifactDataArtifacts, ref artifactsPointer); // UserArtifactData.Artifacts

                var artifactCount = 0;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + RaidStaticInformation.ListCount, ref artifactCount); // List<Artifact>.Count

                var pointers = new List<IntPtr>();
                if (artifactCount > 0)
                {
                    var arrayPointer = artifactsPointer;
                    NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x10, ref arrayPointer); // List<Artifact>._array

                    var ptrs = new IntPtr[artifactCount];
                    NativeWrapper.ReadProcessMemoryArray(handle, arrayPointer + 0x20, ptrs);
                    pointers.AddRange(ptrs);

                }

                if (artifactCount == 0)
                {
                    // This means it's in external storage instead which is in a concurrent dictionary (teh sucks)
                    NativeWrapper.ReadProcessMemory(handle, gameAssembly.BaseAddress + RaidStaticInformation.ExternalStorageAddress, ref klass);

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
                    if (bucketCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, buckets + 0x20, nodes);

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
                    NativeWrapper.ReadProcessMemory(handle, bonusesPointer + 0x18, ref bonusCount);
                    NativeWrapper.ReadProcessMemory(handle, bonusesPointer + 0x10, ref bonusesPointer);

                    artifact.SecondaryBonuses = new List<ArtifactBonus>();

                    var bonuses = new IntPtr[bonusCount];
                    if (bonusCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, bonusesPointer + 0x20, bonuses, 0, bonuses.Length);

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

#endregion

#region Hero Extraction

                var heroesDataPointer = heroesWrapper;
                NativeWrapper.ReadProcessMemory(handle, heroesDataPointer + RaidStaticInformation.HeroesWrapperHeroData, ref heroesDataPointer); // HeroesWrapperReadOnly.HeroData
                NativeWrapper.ReadProcessMemory(handle, heroesDataPointer + RaidStaticInformation.UserHeroDataHeroById, ref heroesDataPointer); // UserHeroData.HeroById

                var count = 0;
                NativeWrapper.ReadProcessMemory(handle, heroesDataPointer + RaidStaticInformation.DictionaryCount, ref count); // Dictionary<int, Hero>.Count
                NativeWrapper.ReadProcessMemory(handle, heroesDataPointer + RaidStaticInformation.DictionaryEntries, ref heroesDataPointer); // Dictionary<int, Hero>.entries

                var heroStruct = new HeroStruct();
                var heroMasteriesStruct = new HeroMasteryDataStruct();
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
                        NativeWrapper.ReadProcessMemory(handle, masteriesPtr + 0x18, ref masteryCount);
                        NativeWrapper.ReadProcessMemory(handle, masteriesPtr + 0x10, ref masteriesPtr);
                    }

                    var masteries = new int[masteryCount];
                    if (masteryCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, masteriesPtr + 0x20, masteries, 0, masteries.Length);
                    hero.Masteries.AddRange(masteries);

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

#endregion

#region Hero Artifact Extraction

                var artifactsByHeroIdPtr = heroesWrapper;
                NativeWrapper.ReadProcessMemory(handle, artifactsByHeroIdPtr + RaidStaticInformation.HeroesWrapperArtifactData, ref artifactsByHeroIdPtr); // HeroesWrapperReadOnly.ArtifactData
                NativeWrapper.ReadProcessMemory(handle, artifactsByHeroIdPtr + RaidStaticInformation.UserArtifactArtifactDataByHeroId, ref artifactsByHeroIdPtr); // UserArtifactData.ArtifactDataByHeroId

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
                NativeWrapper.ReadProcessMemory(handle, userWrapper + RaidStaticInformation.UserWrapperArena, ref arenaPtr);

                ArenaLeagueId arenaLeague = 0;
                NativeWrapper.ReadProcessMemory(handle, arenaPtr + RaidStaticInformation.ArenaWrapperLeagueId, ref arenaLeague);

#endregion

#region Great Hall Extraction

                var villageDataPointer = IntPtr.Zero;
                NativeWrapper.ReadProcessMemory(handle, userWrapper + RaidStaticInformation.UserWrapperCapitol, ref villageDataPointer); // UserWrapper.Capitol
                NativeWrapper.ReadProcessMemory(handle, villageDataPointer + RaidStaticInformation.CapitolWrapperVillageData, ref villageDataPointer); // Capitol.VillageData

                var bonusLevelPointer = IntPtr.Zero;
                NativeWrapper.ReadProcessMemory(handle, villageDataPointer + RaidStaticInformation.UserVillageDataCapitolBonusLevelByStatByElement, ref bonusLevelPointer); // UserVillageData.UserVillageDataCapitolBonusLevelByStatByElement

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
                NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + RaidStaticInformation.UserWrapperShards, ref shardsDataPointer); // UserWrapper.ShardWrapperReadOnly
                NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + RaidStaticInformation.ShardWrapperData, ref shardsDataPointer); // ShardWrapperReadOnly.UserShardData

                var shardSummonDataPointer = shardsDataPointer;
                NativeWrapper.ReadProcessMemory(handle, shardSummonDataPointer + RaidStaticInformation.ShardSummonData, ref shardSummonDataPointer); // UserShardData.SummonResults

                // Read the shard count data
                NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + RaidStaticInformation.ShardData, ref shardsDataPointer); 

                var shardCount = 0;
                NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + RaidStaticInformation.ListCount, ref shardCount);

                List<IntPtr> shardPointers = new List<IntPtr>();
                if (shardCount > 0)
                {
                    var arrayPointer = shardsDataPointer;
                    NativeWrapper.ReadProcessMemory(handle, shardsDataPointer + 0x10, ref arrayPointer); 

                    var ptrs = new IntPtr[shardCount];
                    NativeWrapper.ReadProcessMemoryArray(handle, arrayPointer + 0x20, ptrs);
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
                    NativeWrapper.ReadProcessMemory(handle, shardSummonDataPointer + 0x10, ref arrayPointer);

                    var ptrs = new IntPtr[shardSummonCount];
                    NativeWrapper.ReadProcessMemoryArray(handle, arrayPointer + 0x20, ptrs);
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

                return new AccountDump
                {
                    Artifacts = artifacts,
                    Heroes = heroes,
                    ArenaLeague = arenaLeague.ToString(),
                    GreatHall = greatHall,
                    Shards = shards
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
            if (!process.MainModule.FileName.Contains(RaidStaticInformation.ExpectedRaidVersion))
            {
                throw new Exception("Raid has been updated and needs a newer version of RaidExtractor");
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
