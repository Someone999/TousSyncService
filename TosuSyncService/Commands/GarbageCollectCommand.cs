using HsManCommand;
using HsManCommand.Attributes;
using HsManCommand.Contexts;
using HsManCommand.Results;

namespace TosuSyncService.Commands;

[Command("gc")]
public class GarbageCollectCommand : ICommand
{
    public Task<ICommandExecutionResult> Execute(IExecutionData executeContext, string commandName, string[] args)
    {
        GC.Collect();
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public string GetHelp() => "Force system gc when high memory";


    public string GetUsage() => "/gc";   

    public Task<ICommandExecutionResult> ExceptionHandler(Exception exception)
    {
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public bool AutoHandleException => true;
}