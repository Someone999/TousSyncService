namespace TosuSyncService.Native;

[Flags]
public enum MemoryProtectFlags
{
    NoAccess = 0x01,
    ReadOnly = 0x02,
    ReadWrite = 0x04,
    WriteCopy = 0x08,
    Execute = 0x10,
    ExecuteRead = 0x20,
    ExecuteReadWrite = 0x40,
    ExecuteWriteCopy = 0x80,
    Guard = 0x100,
    NoCache = 0x200,
    WriteCombine = 0x400,
    TargetsInvalid = 0x40000000,
    TargetsNoUpdate = TargetsInvalid
}