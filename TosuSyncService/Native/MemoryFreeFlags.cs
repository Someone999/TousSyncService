namespace TosuSyncService.Native;

[Flags]
public enum MemoryFreeFlags
{
    Decommit = 0x00004000,
    Release = 0x00008000,
    CoalescePlaceholders = 0x00000001,
    PreservePlaceholders = 0x00000002
}