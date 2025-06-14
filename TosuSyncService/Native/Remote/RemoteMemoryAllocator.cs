using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TosuSyncService.Native.Remote;

public class RemoteMemoryAllocator(IntPtr processHandle)
{
    public RemoteMemory Alloc(int size)
    {
        var handle = Win32.VirtualAllocEx(processHandle, IntPtr.Zero, size, MemoryAllocFlags.Commit,
            MemoryProtectFlags.ReadWrite);

        if (handle == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        return new RemoteMemory(processHandle, handle, size);
    }
}