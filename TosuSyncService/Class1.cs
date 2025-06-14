using System.IO.MemoryMappedFiles;
using FleeExpressionEvaluator.Evaluator.Context;
using HsManCommand;
using HsManCommand.CommandParsers;
using HsManCommand.Contexts;
using HsManCommand.Manager;
using HsManCommand.Results;
using HsManCommonLibrary.ValueHolders;
using TosuSyncService.DisplayVariables;
using TosuSyncService.DisplayVariables.Smoothed;
using TosuSyncService.EventSystem;
using TosuSyncService.Expressions.Lexer;
using TosuSyncService.IngameOverlay;
using TosuSyncService.IngameOverlay.Configs;
using TosuSyncService.Mmf;
using TosuSyncService.Model;
using TosuSyncService.Native;
using TosuSyncService.Websockets;
using Timer = System.Timers.Timer;

namespace TosuSyncService;

public class Class1
{
    
    private static CommandManager _commandManager = new CommandManager();
    private static readonly CommonCommandParser CommonCommandParser = new CommonCommandParser();

    private static void InputLoop()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (!input.StartsWith('/'))
            {
                continue;
            }

            var commandBody = input[1..];
            var x = CommonCommandParser.Parse(commandBody);
            if (!_commandManager.TryGetCommand(x.CommandName, out var command) || command == null)
            {
                Console.WriteLine($"Command not found: {x.CommandName}");
                continue;
            }

            try
            {
                command.Execute(new CommonExecutionData<TosuWebSocketClient>(Client), x.CommandName, x.Arguments);
            }
            catch (Exception e)
            {
                HandleCommandException(command, e);
            }
        }
    }

    static readonly TosuWebSocketClient Client = new TosuWebSocketClient();
    private static void HandleCommandException(ICommand command, Exception e)
    {
        if (command.AutoHandleException)
        {
            Console.WriteLine(e);
            return;
        }

        var handleResult = command.ExceptionHandler(e).Result;
        var isExceptionHandled = handleResult.Status == CommandStatus.ExceptionHandled;
        var isOk = handleResult.Status == CommandStatus.Success;
        if (!isExceptionHandled && !isOk)
        {
            throw e;
        }
    }
    static void InjectGame()
    {
        var p = Processes.OsuProcess.GetOsuProcess();
        if (p?.Process == null)
        {
            return;
        }

        if (p.IsLazerProcess)
        {
            Console.WriteLine("osu!Lazer is not supported");
            return;
        }
        DllInjector injector = new DllInjector();
        injector.Inject(Path.GetFullPath("libs/x86/overlay.dll"), p.Process.Id);
    }

    static void AfterInit(Dictionary<string, object> data)
    {
        if (data.TryGetValue("plugin", out var pluginObj) && pluginObj is IngameOverlayPlugin plugin)
        {
            plugin.Enable();
            Setting.GlobalConfig.WriteToMmf();
        }
    }

    static void AddVariables(TosuWebSocketClient client)
    {
        EvaluateContext mainContext = EvaluateContext.Default;
        mainContext.SymbolTable.Symbols.Add("data", client.Data);
        mainContext.SymbolTable.Symbols.Add("system", new SystemInfo());
        mainContext.SymbolTable.Symbols.Add("extra", new ExtraInfo(client, mainContext));
        mainContext.SymbolTable.Symbols.Add("session", new GameSession(mainContext));
        mainContext.SymbolTable.Symbols.Add("smoothed", new SmoothedValues(client, mainContext));
#if ENABLE_COMPLEX
        mainContext.SymbolTable.Symbols.Add("complex", new ComputeIntensiveInfo(mainContext));
#endif
        mainContext.Types.Add(typeof(Functions));
        Global.EvaluateContextRegistry.AddContext("default", mainContext);
    }

    static void InitMmf(IValueHolder<TosuData> tosuData, TosuWebSocketClient client)
    {
        MmfManager mmfManager = MmfManager.CreateInstance(tosuData);
        Timer timer = new Timer();
        timer.Interval = 25;
        timer.AutoReset = true;
        timer.Elapsed += (s, e) => mmfManager.UpdateAll();
        timer.Start();
    }

    static Task Main(string[] args)
    {
        var eventManager = Client.EventManager; 
        AddVariables(Client);
        Client.Connect();
        _commandManager.SearchAssembly.Add(typeof(Class1).Assembly);
        _commandManager.ScanCommands();
        IngameOverlayPlugin plugin = new IngameOverlayPlugin(Client.Data, eventManager);

        ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);

        eventManager.GetEvent<TosuEvent<TosuData>>(TosuEventKeys.TosuDataUpdated)?.Bind((_, data) =>
        {
            if (!resetEvent.IsSet)
            {
                resetEvent.Set();
            }
        });

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("plugin", plugin);
        AfterInit(data);
        resetEvent.Wait();
        InitMmf(Client.Data, Client);

        InjectGame();
        
        InputLoop();
        while (true)
        {
            Thread.Sleep(500);
        }
    }
}