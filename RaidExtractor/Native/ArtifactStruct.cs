using System;
using System.Runtime.InteropServices;

namespace RaidExtractor.Native
{
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
}
