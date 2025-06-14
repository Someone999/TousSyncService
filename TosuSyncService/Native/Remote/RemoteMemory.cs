using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TosuSyncService.Native.Remote;

public class RemoteMemory(IntPtr processHandle, IntPtr pointer, int size) : IDisposable
{
    public IntPtr ProcessHandle { get; private set;} = processHandle;
    public IntPtr Pointer { get; private set; } = pointer;
    public int Size { get; private set; } = size;
    private bool _freed;
    private void Free()
    {
        if (ProcessHandle == IntPtr.Zero || Pointer == IntPtr.Zero || _freed)
        {
            return;
        }

        if (!Win32.VirtualFreeEx(ProcessHandle, Pointer, Size, MemoryFreeFlags.Release))
        {
            return;
        }
        
        _freed = true;
        ProcessHandle = IntPtr.Zero;
        Pointer = IntPtr.Zero;
        Size = 0;
    } 

    public void Dispose()
    {
        Free();
    }

    private void EnsureValid()
    {
        if (ProcessHandle == IntPtr.Zero || Pointer == IntPtr.Zero)
        {
            throw new InvalidOperationException("Try to access a invalid memory region.");
        }
    }
    
    public void WriteBytes(byte[] bytes)
    {
       EnsureValid();
       var gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned).AddrOfPinnedObject();
       var realRead = Marshal.AllocHGlobal(4);
       if (!Win32.WriteProcessMemory(ProcessHandle, Pointer, gcHandle, bytes.Length, realRead))
       {
           throw new Win32Exception(Marshal.GetLastWin32Error());
       }

       var realWrote = Marshal.ReadInt32(realRead);
       
       if (realWrote != bytes.Length)
       {
           throw new InvalidOperationException("Failed to read wrote count.");
       }
    }
}

