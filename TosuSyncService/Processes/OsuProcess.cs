using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TosuSyncService.Processes;

public class OsuProcess
{
    
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process2(
        IntPtr hProcess,            // 进程句柄
        out ProcessArchitecture pProcessMachine, // 进程架构类型
        out ProcessArchitecture pNativeMachine   // 本地机器架构类型
    );

    public bool IsLazerProcess { get; private set; }
    public string GamePath { get; private set; } = "";
    public Process? Process { get; private set; }

    public static OsuProcess? GetOsuProcess()
    {
        var processes = Process.GetProcessesByName("osu!");
        if (processes.Length == 0)
        {
            return null;
        }
        
        var firstProcess = processes[0];
        try
        {
            var mainModule = firstProcess.MainModule;
            if (mainModule == null)
            {
                return null;
            }
       
            return new OsuProcess()
            {
                IsLazerProcess = mainModule.FileVersionInfo.ProductName == "osu!(lazer)",
                GamePath = Path.GetDirectoryName(mainModule.FileName) ?? string.Empty,
                Process = firstProcess
            };
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
}