using System.Text.RegularExpressions;
using FleeExpressionEvaluator.Expressions;
using TosuSyncService.Expressions.Lexer;

namespace TosuSyncService.Expressions;

public class RegexExpressionMatcher : IExpressionMatcher
{
    private static readonly Regex ExpressionPattern = new Regex(@"\${([^}]*?)}", RegexOptions.Compiled); 
    public IExpression[] Matches(string expression)
    {
        List<IExpression> captures = new List<IExpression>();
        var matches = ExpressionPattern.Matches(expression);
        if (matches.Count == 0)
        {
            return [];
        }

        foreach (Match match in matches)
        {
            var text = match.Groups[1].Value;
            var parser = ExpressionParser.Instance;
            captures.Add(parser.Parse(text));
        }
        
        return captures.ToArray();
    }
}