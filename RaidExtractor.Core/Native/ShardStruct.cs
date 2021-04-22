using System.Runtime.InteropServices;

namespace RaidExtractor.Core.Native
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ShardStruct
    {
        [FieldOffset(0x10)]
        public ShardType ShardTypeId;
        [FieldOffset(0x14)]
        public int Count;
    }
}
