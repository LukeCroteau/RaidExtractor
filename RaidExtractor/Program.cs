using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;

namespace RaidExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = Process.GetProcessesByName("Raid").FirstOrDefault();
            if (process == null)
            {
                Console.WriteLine("Raid needs to be running before running RaidExtractor");
                return;
            }

            var handle = NativeWrapper.OpenProcess(ProcessAccessFlags.Read, true, process.Id);
            try
            {
                var gameAssembly = process.Modules.OfType<ProcessModule>().FirstOrDefault(m => m.ModuleName == "GameAssembly.dll");
                if (gameAssembly == null)
                {
                    Console.WriteLine("Unable to locate GameAssembly.dll in memory");
                    return;
                }

                var klass = IntPtr.Zero;
                NativeWrapper.ReadProcessMemory(handle, gameAssembly.BaseAddress + 0x2FD67D0, ref klass);

                var appModel = klass;
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x18, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0xC0, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x0, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0xB8, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x8, ref appModel);

                var userWrapper = appModel;
                NativeWrapper.ReadProcessMemory(handle, userWrapper + 0x140, ref userWrapper);

                var artifactsPointer = userWrapper;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x38, ref artifactsPointer);
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x48, ref artifactsPointer);
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x20, ref artifactsPointer);

                var artifactCount = 0;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x18, ref artifactCount);

                var arrayPointer = artifactsPointer;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x10, ref arrayPointer);

                var pointers = new IntPtr[artifactCount];
                NativeWrapper.ReadProcessMemoryArray(handle, arrayPointer + 0x20, pointers);

                var artifacts = new JArray();
                var artifactStruct = new ArtifactStruct();
                var artifactBonusStruct = new ArtifactBonusStruct();
                var bonusValueStruct = new BonusValueStruct();
                foreach (var pointer in pointers)
                {
                    NativeWrapper.ReadProcessMemory(handle, pointer, ref artifactStruct);
                    NativeWrapper.ReadProcessMemory(handle, artifactStruct.PrimaryBonus, ref artifactBonusStruct);
                    NativeWrapper.ReadProcessMemory(handle, artifactBonusStruct.Value, ref bonusValueStruct);

                    var artifact = new JObject();
                    artifacts.Add(artifact);

                    artifact["id"] = artifactStruct.Id;
                    artifact["sellPrice"] = artifactStruct.SellPrice;
                    artifact["price"] = artifactStruct.Price;
                    artifact["level"] = artifactStruct.Level;
                    artifact["isActivated"] = artifactStruct.IsActivated;
                    artifact["kind"] = artifactStruct.KindId.ToString();
                    artifact["rank"] = artifactStruct.RankId.ToString();
                    artifact["rarity"] = artifactStruct.RarityId.ToString();
                    artifact["setKind"] = artifactStruct.SetKindId.ToString();
                    if (artifactStruct.RequiredFraction != HeroFraction.Unknown)
                        artifact["requiredFraction"] = artifactStruct.RequiredFraction.ToString();
                    artifact["isSeen"] = artifactStruct.IsSeen;
                    artifact["failedUpgrades"] = artifactStruct.FailedUpgrades;

                    artifact.Add("primaryBonus", new JObject());
                    artifact["primaryBonus"]["kind"] = artifactBonusStruct.KindId.ToString();
                    artifact["primaryBonus"]["isAbsolute"] = bonusValueStruct.IsAbsolute;
                    artifact["primaryBonus"]["value"] = Math.Round(bonusValueStruct.Value / (double)uint.MaxValue, 2);

                    var bonusesPointer = artifactStruct.SecondaryBonuses;
                    var bonusCount = 0;
                    NativeWrapper.ReadProcessMemory(handle, bonusesPointer + 0x18, ref bonusCount);
                    NativeWrapper.ReadProcessMemory(handle, bonusesPointer + 0x10, ref bonusesPointer);

                    artifact.Add("secondaryBonuses", new JArray());

                    var bonuses = new IntPtr[bonusCount];
                    if (bonusCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, bonusesPointer + 0x20, bonuses, 0, bonuses.Length);

                    foreach (var bonusPointer in bonuses)
                    {
                        var bonus = new JObject();
                        ((JArray)artifact["secondaryBonuses"]).Add(bonus);

                        NativeWrapper.ReadProcessMemory(handle, bonusPointer, ref artifactBonusStruct);
                        NativeWrapper.ReadProcessMemory(handle, artifactBonusStruct.Value, ref bonusValueStruct);

                        bonus["kind"] = artifactBonusStruct.KindId.ToString();
                        bonus["isAbsolute"] = bonusValueStruct.IsAbsolute;
                        bonus["value"] = Math.Round(bonusValueStruct.Value / (double)uint.MaxValue, 2);
                        bonus["enhancement"] = Math.Round(artifactBonusStruct.PowerUpValue / (double)uint.MaxValue, 2);
                        bonus["level"] = artifactBonusStruct.Level;
                    }
                }

                Console.WriteLine(artifacts);
            }
            finally
            {
                NativeWrapper.CloseHandle(handle);
            }
        }
    }

    public enum ArtifactKindId 
    {
        UnknownArtifact = 0,
        Helmet = 1,
        Chest = 2,
        Gloves = 3,
        Boots = 4,
        Weapon = 5,
        Shield = 6,
        Ring = 7,
        Cloak = 8,
        Banner = 9,
        UnknownAccessory = 10,
    }
    public enum ArtifactRankId : int
    {
        Unknown = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
    }
    public enum ArtifactRarityId : int
    {
        Unknown = 0,
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,
        Legendary = 5,
    }
    public enum ArtifactSetKindId : int
    {
        None = 0,
        Hp = 1,
        AttackPower = 2,
        Defense = 3,
        AttackSpeed = 4,
        CriticalChance = 5,
        CriticalDamage = 6,
        Accuracy = 7,
        Resistance = 8,
        LifeDrain = 9,
        DamageIncreaseOnHpDecrease = 10,
        SleepChance = 11,
        BlockHealChance = 12,
        FreezeRateOnDamageReceived = 13,
        Stamina = 14,
        Heal = 15,
        BlockDebuff = 16,
        Shield = 17,
        GetExtraTurn = 18,
        IgnoreDefense = 19,
        DecreaseMaxHp = 20,
        StunChance = 21,
        DotRate = 22,
        ProvokeChance = 23,
        Counterattack = 24,
        CounterattackOnCrit = 25,
        AoeDamageDecrease = 26,
        CooldownReductionChance = 27,
        CriticalHealMultiplier = 28,
        AttackPowerAndIgnoreDefense = 29,
        HpAndHeal = 30,
        ShieldAndAttackPower = 31,
        ShieldAndCriticalChance = 32,
        ShieldAndHp = 33,
        ShieldAndSpeed = 34,
        UnkillableAndSpdAndCrDmg = 35,
        BlockReflectDebuffAndHpAndDef = 36,
        IgnoreCooldown = 1000,
        RemoveDebuff = 1001,
    }
    public enum HeroFraction : int
    {
        Unknown = 0,
        BannerLords = 1,
        HighElves = 2,
        SacredOrder = 3,
        CovenOfMagi = 4,
        OgrynTribes = 5,
        LizardMen = 6,
        Skinwalkers = 7,
        Orcs = 8,
        Demonspawn = 9,
        UndeadHordes = 10,
        DarkElves = 11,
        KnightsRevenant = 12,
        Barbarians = 13,
        NyresanElves = 14,
        AssassinsGuild = 15,
        Dwarves = 16,
    }
    public enum StatKindId : int
    {
        Health = 1,
        Attack = 2,
        Defence = 3,
        Speed = 4,
        Resistance = 5,
        Accuracy = 6,
        CriticalChance = 7,
        CriticalDamage = 8,
        CriticalHeal = 9,
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct ArtifactStruct
    {
        [FieldOffset(0x10)]
        public int Id; 
        [FieldOffset(0x14)]
        public int SellPrice; 
        [FieldOffset(0x18)]
        public int Price;
        [FieldOffset(0x1C)]
        public int Level; 
        [FieldOffset(0x20)]
        public bool IsActivated; 
        [FieldOffset(0x24)]
        public ArtifactKindId KindId;
        [FieldOffset(0x28)]
        public ArtifactRankId RankId;
        [FieldOffset(0x2C)]
        public ArtifactRarityId RarityId; 
        [FieldOffset(0x30)]
        public IntPtr PrimaryBonus; 
        [FieldOffset(0x38)]
        public IntPtr SecondaryBonuses;
        [FieldOffset(0x40)]
        public ArtifactSetKindId SetKindId;
        [FieldOffset(0x44)]
        public HeroFraction RequiredFraction;
        [FieldOffset(0x48)]
        public bool IsSeen;
        [FieldOffset(0x4C)]
        public int FailedUpgrades;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ArtifactBonusStruct
    {
        [FieldOffset(0x10)]
        public StatKindId KindId; 
        [FieldOffset(0x18)]
        public IntPtr Value; 
        [FieldOffset(0x20)]
        public long PowerUpValue; 
        [FieldOffset(0x28)]
        public int Level; 
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct BonusValueStruct
    {
        [FieldOffset(0x10)]
        public bool IsAbsolute;
        [FieldOffset(0x18)]
        public long Value;
    }
}
