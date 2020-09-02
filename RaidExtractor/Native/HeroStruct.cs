using System;
using System.Runtime.InteropServices;

namespace RaidExtractor.Native
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HeroStruct
    {
        [FieldOffset(0x10)]
        public IntPtr HeroType; 
        [FieldOffset(0x18)]
        public int Id; 
        [FieldOffset(0x1C)]
        public int TypeId; 
        [FieldOffset(0x20)]
        public HeroGrade Grade; 
        [FieldOffset(0x24)]
        public int Level; 
        [FieldOffset(0x28)]
        public int Experience; 
        [FieldOffset(0x2C)]
        public int FullExperience; 
        [FieldOffset(0x30)]
        public bool Locked; 
        [FieldOffset(0x31)]
        public bool InStorage; 
        [FieldOffset(0x38)]
        public long RecentBattleTime; 
        [FieldOffset(0x48)]
        public IntPtr BattleStatistics; 
        [FieldOffset(0x50)]
        public IntPtr Skills; 
        [FieldOffset(0x58)]
        public IntPtr MasteryData; 
    }
}
