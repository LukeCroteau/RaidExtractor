using System.Runtime.InteropServices;

namespace RaidExtractor.Core.Native
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ShardSummonData
    {
        [FieldOffset(0x10)]
        public ShardType ShardTypeId;
        [FieldOffset(0x14)]
        public HeroRarity rarity;
        [FieldOffset(0x18)]
        public int pullCount;
        [FieldOffset(0x1C)]
        public int lastHeroId;
    }
}
