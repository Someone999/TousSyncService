namespace TosuSyncService.Expressions;

public interface IExpressionCapture
{
    string Expression { get; }
    Dictionary<string, string> Metadata { get; }
    string? OutputFormat { get; }
}