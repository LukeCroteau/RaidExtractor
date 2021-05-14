using System;
using System.Runtime.InteropServices;

namespace RaidExtractor.Core.Native
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SkillStruct
    {
        [FieldOffset(0x18)]
        public int Id; 
        [FieldOffset(0x1C)]
        public int TypeId; 
        [FieldOffset(0x20)]
        public int Level;
    }
}
