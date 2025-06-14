using System.Runtime.InteropServices;

namespace TosuSyncService.Native;

public static class Win32
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(OpenProcessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryAllocFlags flAllocationType,
        MemoryProtectFlags flProtectFlags);

    [DllImport("kernel32.dll")]
    public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryFreeFlags dwFreeType);

    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateRemoteThread(IntPtr hProcess, int attrib, int size, IntPtr address, IntPtr par,
        int flags, int threadId);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hProcess, string lpName);

    [DllImport("Kernel32.dll")]
    public static extern ulong WaitForSingleObject(IntPtr hHandle, long dwMilliseconds);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr addrMem, IntPtr buffer, int intSize,
        IntPtr lpNumberOfBytesWritten);

    [DllImport("Kernel32.dll")]
    public static extern bool GetExitCodeThread(IntPtr hThread, IntPtr lpExitCode);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);
    
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr LoadLibraryW(string module);
}