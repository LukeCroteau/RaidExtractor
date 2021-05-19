namespace RaidExtractor.Core.Native
{
    class RaidStaticInformation
    {
        public static string ExpectedRaidVersion = "\\240\\";
        public static int MemoryLocation = 57020984;
        public static int ExternalStorageAddress = 57155120;

        public static int UserVillageDataCapitolBonusLevelByStatByElement = 0x30; // UserVillageData.UserVillageDataCapitolBonusLevelByStatByElement

        public static int ShardData = 0x10; // ShardWrapperReadOnly.UserShardData.Shards
        public static int ShardSummonData = 0x18; // ShardWrapperReadOnly.UserShardData.SummonResults

        public static int UserHeroDataHeroById = 0x18; // UserHeroData.HeroById
        public static int UserHeroDataBattlePresets = 0x28; // UserHeroData.BattlePresets

        public static int UserArtifactDataArtifacts = 0x28; // UserArtifactData.Artifactsa
        public static int UserArtifactArtifactDataByHeroId = 0x30; // UserArtifactData.ArtifactDataByHeroId

        public static int AppModelUserWrapper = 0x168; // AppModel._userWrapper

        public static int UserWrapperHeroes = 0x28; // UserWrapper.Heroes
        public static int UserWrapperShards = 0x70; // UserWrapper.Shards
        public static int UserWrapperArena = 0xB0; // UserWrapper.Arena
        public static int UserWrapperCapitol = 0xC8; // UserWrapper.Capitol

        public static int CapitolWrapperVillageData = 0x18; // CapitolWrapperReadOnly.VillageData

        public static int ShardWrapperData = 0x18; // ShardWrapperReadOnly

        public static int HeroesWrapperArtifactData = 0x60; // HeroesWrapperReadOnly.ArtifactData
        public static int HeroesWrapperHeroData = 0x50; // HeroesWrapperReadOnly.HeroData

        public static int ArenaWrapperLeagueId = 0x40; // ArenaWrapperReadOnly.LeagueId

        public static int DictionaryEntries = 0x18; // Dictionary.Entries
        public static int DictionaryCount = 0x20; // Dictionary.Count
        public static int ListIndexArray = 0x10; // Offset to array of element pointers.
        public static int ListCount = 0x18; // List.Count
        public static int ListElementPointerArray = 0x20; // Offset from ListIndexArray to start of element pointers.
    }
}
