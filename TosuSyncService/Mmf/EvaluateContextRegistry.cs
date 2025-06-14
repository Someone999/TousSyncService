

using FleeExpressionEvaluator.Evaluator.Context;

namespace TosuSyncService.Mmf;

public class EvaluateContextRegistry
{
    Dictionary<string, EvaluateContext> _symbolTables = new Dictionary<string, EvaluateContext>();

    public void AddContext(string symbol, EvaluateContext context)
    {
        _symbolTables.Add(symbol, context);
    }

    public EvaluateContext? GetContext(string name)
    {
        return _symbolTables.GetValueOrDefault(name);
    }
}