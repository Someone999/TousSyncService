using System.Runtime.Versioning;
using HsManCommand;
using HsManCommand.Attributes;
using HsManCommand.Contexts;
using HsManCommand.Results;
using HsManCommonLibrary.ValueHolders;
using TosuSyncService.Mmf;
using TosuSyncService.Model;

namespace TosuSyncService.Commands;

[SupportedOSPlatform("windows")]
[Command("mmf")]
public class MmfCommand : ICommand
{
    static readonly Task<ICommandExecutionResult> SuccessResult 
        = Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));


    void PerformStateCommand(string[] args)
    {
        var t = Global.DefaultContext.SymbolTable.Symbols["data"];
        if (t is not IValueHolder<TosuData> gosuDataHolder)
        {
            return;
        }
        
        if (args.Length < 2)
        {
            return;
        }

        var operation = args[1];
        var updater = MmfManager.GetInstance(gosuDataHolder).Updater;
        switch (operation)  
        {
            case "stop":
                updater.StopUpdate();
                break;
            case "start":
                updater.StartUpdate();
                break;
        }
    }
    
    
    void PerformIntervalCommand(string[] args)
    {
        var t = Global.DefaultContext.SymbolTable.Symbols["data"];
        if (t is not IValueHolder<TosuData> gosuDataHolder)
        {
            return;
        }
        
        if (args.Length < 2)
        {
            return;
        }

        var intervalStr = args[1];
        var updater = MmfManager.GetInstance(gosuDataHolder).Updater;
        updater.UpdateInterval = double.Parse(intervalStr);
    }
    
    void PerformCommand(string[] args)
    {
        switch (args[0])
        {
            case "state":
                PerformStateCommand(args);
                break;
            case "interval":
                PerformIntervalCommand(args);
                break;
        }
    }
    
    public Task<ICommandExecutionResult> Execute(IExecutionData executeContext, string commandName, string[] args)
    {
        PerformCommand(args);
        return SuccessResult;
    }

    public string GetHelp()
    {
        return "/mmf state <stop|start>\n/mmf interval <updateInterval>";
    }

    public string GetUsage()
    {
        return "/mmf state <stop|start>\n/mmf interval <updateInterval>";
    }

    public Task<ICommandExecutionResult> ExceptionHandler(Exception exception)
    {
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public bool AutoHandleException => true;
}