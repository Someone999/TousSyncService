using FleeExpressionEvaluator.Evaluator.Context;
using TosuSyncService.BeatmapCaches;
using SymbolTable = FleeExpressionEvaluator.Evaluator.Context.SymbolTable;

namespace TosuSyncService.Mmf;

public static class Global
{
    public static EvaluateContextRegistry EvaluateContextRegistry { get; } = new EvaluateContextRegistry();

    public static EvaluateContext DefaultContext =>
        EvaluateContextRegistry.GetContext("default") ?? throw new Exception();
    public static BeatmapMemoryCache BeatmapMemoryCache { get; } = new BeatmapMemoryCache();
    public static DifficultyAttributeCache DifficultyAttributeCache { get; private set; } = new DifficultyAttributeCache();
    public static bool EnableRecordSave { get; set; }
}