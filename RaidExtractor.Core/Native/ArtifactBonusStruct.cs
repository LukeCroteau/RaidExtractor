using System;
using System.Runtime.InteropServices;

namespace RaidExtractor.Core.Native
{
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
}
