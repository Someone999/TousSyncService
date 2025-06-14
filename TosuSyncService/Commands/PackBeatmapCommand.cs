using System.Diagnostics;
using HsManCommand;
using HsManCommand.Attributes;
using HsManCommand.Contexts;
using HsManCommand.Results;
using osuToolsV2.Reader;
using osuToolsV2.Utils;
using TosuSyncService.Websockets;

namespace TosuSyncService.Commands;

[Command("pack")]
public class PackBeatmapCommand : ICommand<TosuWebSocketClient>
{
    public Task<ICommandExecutionResult> Execute(IExecutionData<TosuWebSocketClient> executeContext, string commandName, string[] args)
    {
        if (commandName != "pack")
        {
            return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Failed));
        }
        
        Console.WriteLine("Packing...");

        var client = executeContext.Data;
        if (client == null)
        {
            return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Failed));
        }

        var gosuDataHolder = client.Data;
        if (!gosuDataHolder.IsInitialized() || gosuDataHolder.Value == null)
        {
            return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Failed));
        }

        var gosuData = gosuDataHolder.Value;
        var currentBeatmapHash = gosuData.Beatmap.Checksum;
        OsuBeatmapDbObjectReader reader = new OsuBeatmapDbObjectReader();
        var db = reader.Read();
        var beatmap = db.Beatmaps.FindByMd5(currentBeatmapHash);
        if (beatmap == null)
        {
            return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Failed));
        }
        
        var targetFileName = $"{beatmap.FolderName}.osz";
        
        
        try
        {
            var prefix = @"D:\PackedBeatmaps\";
            var fullPath = Path.Combine(prefix, targetFileName);
            prefix = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(prefix))
            {
                return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Failed));
            }
            
            if (!Directory.Exists(prefix))
            {
                Directory.CreateDirectory(prefix);
            }

            var stopwatch = Stopwatch.StartNew();
            beatmap.PackBeatmap(fullPath, overwrite: false);
            stopwatch.Stop();
            Console.WriteLine($"Used time: {stopwatch.Elapsed}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.ExceptionHandled));
        }
        
        Console.WriteLine("Pack completed.");
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public Task<ICommandExecutionResult> Execute(IExecutionData executeContext, string commandName, string[] args)
    {
        return executeContext is not IExecutionData<TosuWebSocketClient> clientData 
            ? Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Failed)) 
            : Execute(clientData, commandName, args);
    }

    public string GetHelp()
    {
        return "Pack current beatmap.";
    }

    public string GetUsage()
    {
        return "/pack";
    }

    public Task<ICommandExecutionResult> ExceptionHandler(Exception exception)
    {
        return Task.FromResult<ICommandExecutionResult>(new CommonExecutionResult(CommandStatus.Success));
    }

    public bool AutoHandleException => true;
}