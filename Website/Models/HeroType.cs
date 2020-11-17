using System.Collections.Generic;

namespace Website.Models
{
    public class HeroType
    {
        public int Id { get; set; }
        public Translation Name { get; set; }
        public Element Element { get; set; }
        public HeroRole Role { get; set; }
        public HeroFraction Fraction { get; set; }
        public HeroRarity Rarity { get; set; }
        public BattleStats BaseStats { get; set; }
        public int[] SkillTypeIds { get; set; }
        public LeaderSkill LeaderSkill { get; set; }
        public double SummonWeight { get; set; }
        public ReadyStatus Status { get; set; }
    }

    public class LeaderSkill
    {
        public StatKindId StatKindId { get; set; }
        public bool IsAbsolute { get; set; }
        public long Amount { get; set; }
        public AreaTypeId? Area { get; set; }
        public Element? Element { get; set; }
    }
    public enum AreaTypeId : int
    {
        Story = 1,
        Dungeon = 2,
        Arena = 3,
        AllianceBoss = 4,
        Fractions = 5,
        Arena3X3 = 6,
    }

    public enum StatKindId : int
    {
        Health = 1,
        Attack = 2,
        Defense = 3,
        Speed = 4,
        Resistance = 5,
        Accuracy = 6,
        CriticalChance = 7,
        CriticalDamage = 8,
        CriticalHeal = 9,
    }

    public class BattleStats
    {
        public long Health { get; set; }
        public long Attack { get; set; }
        public long Defence { get; set; }
        public long Speed { get; set; }
        public long Resistance { get; set; }
        public long Accuracy { get; set; }
        public long CriticalChance { get; set; }
        public long CriticalDamage { get; set; }
        public long CriticalHeal { get; set; }
    }

    public enum ReadyStatus : int
    {
        Unknown = 0,
        Balance = 10,
        Rebalance = 15,
        Preview3D = 20,
        SkillsSetup = 30,
        PreChecked = 35,
        Checked = 40,
        Quarantine = 50,
    }

    public enum Element : int
    {
        Magic = 1,
        Force = 2,
        Spirit = 3,
        Void = 4,
    }

    public enum HeroRole : int
    {
        Attack = 0,
        Defense = 1,
        Health = 2,
        Support = 3,
        Evolve = 4,
        Xp = 5,
    }

    public enum HeroRarity : int
    {
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,
        Legendary = 5,
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

}
