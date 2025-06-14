namespace TosuSyncService.Processes;

public enum ProcessArchitecture : ushort
{
    // Unknown architecture
    Unknown = 0,

    // 32-bit architecture (x86)
    X86 = 0x00000001,

    // 64-bit architecture (x64)
    X64 = 0x00000002,

    // ARM architecture (32-bit ARM)
    Arm = 0x00000003,

    // ARM 64-bit architecture (ARM64)
    Arm64 = 0x00000004
}