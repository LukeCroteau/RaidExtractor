using System.Runtime.InteropServices;

namespace RaidExtractor.Core.Native
{
    [StructLayout(LayoutKind.Explicit)]
    public struct BonusValueStruct
    {
        [FieldOffset(0x10)]
        public bool IsAbsolute;
        [FieldOffset(0x18)]
        public long Value;
    }
}
