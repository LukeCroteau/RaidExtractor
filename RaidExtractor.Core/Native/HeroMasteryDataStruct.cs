using System;
using System.Runtime.InteropServices;

namespace RaidExtractor.Core.Native
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HeroMasteryDataStruct
    {
        [FieldOffset(0x10)]
        public IntPtr CurrentAmount;
        [FieldOffset(0x18)]
        public IntPtr TotalAmount;
        [FieldOffset(0x20)]
        public IntPtr Masteries;
        [FieldOffset(0x28)]
        public int ResetCount;
    }
}
