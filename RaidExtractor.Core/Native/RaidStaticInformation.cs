namespace RaidExtractor.Core.Native
{
    class RaidStaticInformation
    {
        public static string ExpectedRaidVersion = "\\237\\";
        public static int MemoryLocation = 56875040;
        public static int ExternalStorageAddress = 57009952;

        public static int UserHeroDataHeroById = 0x18; // UserHeroData.HeroById

        public static int UserArtifactDataArtifacts = 0x28; // UserArtifactData.Artifactsa
        public static int UserArtifactArtifactDataByHeroId = 0x30; // UserArtifactData.ArtifactDataByHeroId

        public static int AppModelUserWrapper = 0x158; // AppModel._userWrapper

        public static int UserWrapperHeroes = 0x28; // UserWrapper.Heroes
        public static int UserWrapperShards = 0x70; // UserWrapper.Shards
        public static int UserWrapperArena = 0xB0; // UserWrapper.Arena
        public static int UserWrapperCapitol = 0xC8; // UserWrapper.Capitol

        public static int HeroesWrapperArtifactData = 0x60; // HeroesWrapperReadOnly.ArtifactData
        public static int HeroesWrapperHeroData = 0x50; // HeroesWrapperReadOnly.HeroData

        public static int DictionaryCount = 0x20; // Dictionary.Count
    }
}
