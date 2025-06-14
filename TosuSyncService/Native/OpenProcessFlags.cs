namespace TosuSyncService.Native;

[Flags]
public enum OpenProcessFlags : long
{
    Terminate = 0x01,
    CreateThread = 0x02,
    VirtualMemoryOperation = 0x08,
    ReadVirtualMemory = 0x010,
    WriteVirtualMemory = 0x020,
    DuplicateHandle = 0x40,
    CreateProcess = 0x80,
    SetQuota = 0x100,
    SetInformation = 0x200,
    QueryInformation = 0x400,
    QueryLimitedInformation = 0x1000,
    Synchronize = 0x00100000L,
    AllAccess = 0x000F0000L | Synchronize | 0xFFFF
}