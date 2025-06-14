using System.Diagnostics;
using System.Reflection;
using HsManCommand;
using HsManCommand.Attributes;
using HsManCommand.Contexts;
using HsManCommand.Results;

namespace TosuSyncService.Commands;

[Command("restart")]
public class RestartCommand : ICommand
{
    public Task<ICommandExecutionResult> Execute(IExecutionData executeContext, string commandName, string[] args)
    {
        var currentAsm = Assembly.GetExecutingAssembly();
        var currentLocation = currentAsm.Location;
        var extension = Path.GetExtension(currentLocation);
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WorkingDirectory = Path.GetDirectoryName(currentLocation);
        startInfo.FileName = currentLocation;
        if (extension != ".exe")
        {
            var fileName = Path.GetFileNameWithoutExtension(currentLocation);
            startInfo.FileName = fileName + ".exe";
        }
        
        Process.Start(startInfo);
        Environment.Exit(0);
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public string GetHelp()
    {
        return "Restart the program.";
    }

    public string GetUsage()
    {
        return "/restart";
    }

    public Task<ICommandExecutionResult> ExceptionHandler(Exception exception)
    {
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public bool AutoHandleException => true;
}