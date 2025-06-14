using HsManCommand;
using HsManCommand.Attributes;
using HsManCommand.Contexts;
using HsManCommand.Results;
using TosuSyncService.Websockets;

namespace TosuSyncService.Commands;

/*[Command("save")]
public class SaveChangeCommand : ICommand<TosuWebSocketClient>
{
    public Task<ICommandExecutionResult> Execute(IExecutionData<TosuWebSocketClient> executeContext, string commandName, string[] args)
    {
        try
        {
            Program.Save();
            return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
        }
        catch (Exception e)
        {
            return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.ExceptionHandled,
                exception: e));
        }
    }

    public Task<ICommandExecutionResult> Execute(IExecutionData executeContext, string commandName, string[] args)
    {
        return executeContext is not IExecutionData<GosuWebsocketClient> data
            ? Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Failed))
            : Execute(data, commandName, args);
    }

    public string GetHelp()
    {
        return "Save changes";
    }

    public string GetUsage()
    {
        return "/save";
    }

    public Task<ICommandExecutionResult> ExceptionHandler(Exception exception)
    {
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public bool AutoHandleException => true;
}*/