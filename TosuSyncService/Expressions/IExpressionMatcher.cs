using FleeExpressionEvaluator.Expressions;

namespace TosuSyncService.Expressions;

public interface IExpressionMatcher
{
    IExpression[] Matches(string expression);
}