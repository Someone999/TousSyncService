using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TosuSyncService.Native;

public class NativeDllInvoker
{
    private IntPtr _moduleHandle;

    private NativeDllInvoker(IntPtr moduleHandle)
    {
        _moduleHandle = moduleHandle;
    }

    public NativeProc GetProcedure(string procName)
    {
        var proc = Win32.GetProcAddress(_moduleHandle, procName);
        return new NativeProc(proc);
    }

    public static NativeDllInvoker Create(string moduleName)
    {
        var moduleHandle = Win32.LoadLibraryW(moduleName);
        if (moduleHandle == IntPtr.Zero)
        {
            throw new FileLoadException("Failed to load unmanaged library.",
                new Win32Exception(Marshal.GetLastWin32Error()));
        }

        return new NativeDllInvoker(moduleHandle);
    }

    public static NativeDllInvoker Kernel32 { get; } = Create("Kernel32.dll");
    public static NativeDllInvoker User32 { get; } = Create("User32.dll");
}