using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using System.Text;
using FleeExpressionEvaluator.Evaluator;
using FleeExpressionEvaluator.Evaluator.Context;
using osuToolsV2.Game.Legacy;
using Realms;
using TosuSyncService.Expressions;
using TosuSyncService.Model;
using TosuSyncService.Output.Mmf;

namespace TosuSyncService.Mmf;

[SupportedOSPlatform("windows")]
public class ExpressionMmfItem : MmfItem
{
    private readonly EvaluateContext _context;
    private readonly ExpressionManager _expressionManager;
    private string _mmfName = "";

    public ExpressionMmfItem(string mmfName, string patternFileName, string symbolTableName) : base(mmfName)
    {
        _context = Global.EvaluateContextRegistry.GetContext(symbolTableName) ??
                   throw new InvalidOperationException("Specified symbol table not registered.");
        _expressionManager = new ExpressionManager(patternFileName);
        _expressionManager.StartWatchFileWrite();
        _expressionManager.ConfigUpdated += (_, _) => OutputPattern = _expressionManager.OutputPattern;
        SetMmfName(mmfName);
    }

    public override string MmfName
    {
        get => _mmfName;
        set => SetMmfName(value);
    }

    private void SetMmfName(string name)
    {
        lock (_readerWriterLocker)
        {
            SetMmf(MmfInstanceManager.Default.CreateOrGetMmfInstance(name, 2048));
        }
    }


    private void SetMmf(MmfInstance instance)
    {
        if (MmfOutputMethod == null)
        {
            MmfOutputMethod = new MmfOutputMethod(instance);
            _mmfName = instance.MmfName;
            return;
        }

        if (MmfOutputMethod.IsSameMmfInstance(instance))
        {
            return;
        }

        MmfOutputMethod.ReplaceMmfInstance(instance);
        _mmfName = instance.MmfName;
    }

    public override void Init()
    {
        UpdateConfigFile();
    }

    private void UpdateConfigFileNoLock()
    {
        _expressionManager.Update();
    }


    private void UpdateNoLock()
    {
        if (MmfOutputMethod == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(OutputPattern))
        {
            UpdateConfigFile();
        }
        

        StringBuilder builder = new StringBuilder(OutputPattern);
        ExpressionReplacer.Instance.Replace(_expressionManager.Captures, builder, _context);
        builder.Append("\0\0");
        var buffer = Encoding.UTF8.GetBytes(builder.ToString());
        MmfOutputMethod.Rewind();
        MmfOutputMethod.Write(buffer);
    }

    private readonly object _readerWriterLocker = new object();

    private void UpdateConfigFile()
    {
        lock (_readerWriterLocker)
        {
            UpdateConfigFileNoLock();
            OutputPattern = _expressionManager.OutputPattern;
        }
    }

    public override void Update(TosuData tosuData)
    {
        lock (_readerWriterLocker)
        {
            if ((tosuData.Play.Mods.Value & LegacyGameMod.Cinema) != 0)
            {
                return;
            }
            
            UpdateNoLock();
        }
    }


    public override void Dispose()
    {
        _expressionManager.StopWatchFileWrite();
        MmfOutputMethod?.Dispose();
        GC.SuppressFinalize(this);
    }
}