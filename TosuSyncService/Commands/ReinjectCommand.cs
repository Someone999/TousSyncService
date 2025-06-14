using System.Diagnostics;
using HsManCommand;
using HsManCommand.Attributes;
using HsManCommand.Contexts;
using HsManCommand.Results;
using TosuSyncService.Native;

namespace TosuSyncService.Commands;

[Command("reinject", "reinj")]
public class ReinjectCommand : ICommand
{
    public Task<ICommandExecutionResult> Execute(IExecutionData executeContext, string commandName, string[] args)
    {
#if ENABLE_INJECTOR
        DllInjector dllInjector = new DllInjector();
        var processes = Process.GetProcessesByName("osu!");
        while (processes.Length == 0)
        {
            Thread.Sleep(500);
            processes = Process.GetProcessesByName("osu!");
            Console.WriteLine("Waiting for process...");
        }
            
        dllInjector.Inject("./libs/Overlay.dll", processes[0].Id);
        Console.WriteLine("Injected.");
#else
        Console.WriteLine("Injector is disabled in this version.");
#endif
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public string GetHelp() => "Re-inject overlay dll into osu!";


    public string GetUsage() => "/reinject, /reinj";   

    public Task<ICommandExecutionResult> ExceptionHandler(Exception exception)
    {
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public bool AutoHandleException => true;
}