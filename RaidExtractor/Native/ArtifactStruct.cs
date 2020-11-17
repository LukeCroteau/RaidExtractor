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
        public int Revision;
        [FieldOffset(0x18)]
        public IntPtr UpgradeTimePtr; 
        [FieldOffset(0x20)]
        public long UpgradeTime;
        [FieldOffset(0x28)]
        public int SellPrice;
        [FieldOffset(0x2C)]
        public int Price;
        [FieldOffset(0x30)]
        public int Level; 
        [FieldOffset(0x34)]
        public bool IsActivated; 
        [FieldOffset(0x38)]
        public ArtifactKindId KindId;
        [FieldOffset(0x3c)]
        public ArtifactRankId RankId;
        [FieldOffset(0x40)]
        public ArtifactRarityId RarityId; 
        [FieldOffset(0x48)]
        public IntPtr PrimaryBonus; 
        [FieldOffset(0x50)]
        public IntPtr SecondaryBonuses;
        [FieldOffset(0x58)]
        public ArtifactSetKindId SetKindId;
        [FieldOffset(0x5C)]
        public HeroFraction RequiredFraction;
        [FieldOffset(0x60)]
        public bool IsSeen;
        [FieldOffset(0x64)]
        public int FailedUpgrades;
    }
}
