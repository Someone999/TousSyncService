using TosuSyncService.Mmf;

namespace TosuSyncService;

public static class Functions
{
    public static object Set(string name, object value)
    {
        return Global.EvaluateContextRegistry.GetContext("default")!.SymbolTable.Symbols[name] = value;
    }
}