using FleeExpressionEvaluator.Expressions;

namespace TosuSyncService.Expressions;

public class TosuExpression(string fullText, string expressionText) : IExpression
{
    public Dictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

    public string? OutputFormat { get; internal set; }

    public string FullText { get; } = fullText;
    public string ExpressionText { get; } = expressionText;
}