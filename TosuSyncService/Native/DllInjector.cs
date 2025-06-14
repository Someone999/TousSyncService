using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TosuSyncService.Native.Remote;

namespace TosuSyncService.Native;

public class DllInjector : ILibraryInjector
{
    private bool ComparePaths(string pathA, string pathB)
    {
        var currentSeries = SystemPlatform.Current;
        var normalizedPathA = NormalizeDirSeparator(Path.GetFullPath(pathA));
        var normalizedPathB = NormalizeDirSeparator(Path.GetFullPath(pathB));

        return currentSeries switch
        {
            PlatformType.Windows => CompareWindowsPaths(normalizedPathA, normalizedPathB),
            PlatformType.LinuxFamily or PlatformType.UnixFamily or PlatformType.AppleFamily => 
                CompareUnixPaths(normalizedPathA, normalizedPathB),
            _ => throw new PlatformNotSupportedException()
        };
    }

    private string NormalizeDirSeparator(string path)
    {
        return Path.DirectorySeparatorChar == '\\'
            ? path.Replace('/', '\\')
            : path.Replace('\\', '/');
    }

    private bool ComparePathsInternal(string pathA, string pathB, bool caseSensitive)
    {
        var partA = pathA.Split(Path.DirectorySeparatorChar);
        var partB = pathB.Split(Path.DirectorySeparatorChar);

        if (partA.Length != partB.Length)
        {
            return false;
        }

        var compareOption = caseSensitive
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        return !partA.Where((t, i) => !t.Equals(partB[i], compareOption)).Any();
    }

    private bool CompareWindowsPaths(string pathA, string pathB) =>
        pathA[0] == pathB[0] && ComparePathsInternal(pathA, pathB, false);

    private bool CompareUnixPaths(string pathA, string pathB) => ComparePathsInternal(pathA, pathB, true);


    private bool IsModuleLoaded(Process process, string dllPath)
    {
        try
        {
            var modules = process.Modules;
            foreach (ProcessModule processModule in modules)
            {
                var moduleFullPath = processModule.FileName;
                var currentModulePath = Path.GetFullPath(dllPath);

                if (ComparePaths(moduleFullPath, currentModulePath))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Inject(string libraryPath, int pid)
    {
        var process = Process.GetProcessById(pid);
        if (IsModuleLoaded(process, libraryPath))
        {
            return;
        }

        var processHandle = process.Handle;

        if (processHandle.ToInt32() is -1 or 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        var proc = NativeDllInvoker.Kernel32.GetProcedure("LoadLibraryW");
        if (!proc.IsValid)
        {
            throw new InvalidOperationException("Failed to get proc address");
        }

        var bytes = Encoding.Unicode.GetBytes(Path.GetFullPath(libraryPath));
        RemoteMemoryAllocator allocator = new RemoteMemoryAllocator(processHandle);
        var remoteMemory = allocator.Alloc(bytes.Length);
        remoteMemory.WriteBytes(bytes);

        var t = Win32.CreateRemoteThread(processHandle, 0, 0, proc.Address, remoteMemory.Pointer, 0, 0);
        if (t == IntPtr.Zero)
        {
            throw new Exception("Failed to create thread.");
        }

        Win32.WaitForSingleObject(t, 0xFFFFFFFF);
        remoteMemory.Dispose();
        Win32.CloseHandle(t);
    }
}