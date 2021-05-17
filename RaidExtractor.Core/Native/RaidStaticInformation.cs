namespace RaidExtractor.Core.Native
{
    class RaidStaticInformation
    {
        public static int DictionaryEntries = 0x18; // Dictionary.Entries
        public static int DictionaryCount = 0x20; // Dictionary.Count
        public static int ListIndexArray = 0x10; // Offset to array of element pointers.
        public static int ListCount = 0x18; // List.Count
        public static int ListElementPointerArray = 0x20; // Offset from ListIndexArray to start of element pointers.
    }
}
