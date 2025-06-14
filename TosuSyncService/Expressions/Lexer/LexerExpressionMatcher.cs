using FleeExpressionEvaluator.Expressions;
using FleeExpressionEvaluator.Lexers.Rules;
using HsManLexer.Lexers;
using HsManLexer.Rules;
using HsManLexer.Tokens;

namespace TosuSyncService.Expressions.Lexer;

public class LexerExpressionMatcher : IExpressionMatcher
{
    private readonly RuleBasedLexer _lexer = new RuleBasedLexer();
    private readonly ExpressionParser _parser = ExpressionParser.Instance;
    private LexerExpressionMatcher()
    {
        _lexer.IgnoreUnknownCharacters = true;
        var fullExpressionRule = new FullExpressionLexerRule();
        _lexer.Rules.Add(fullExpressionRule);
        
    }

    public IExpression[] Matches(string expression)
    {
        List<IExpression> captures = new List<IExpression>();
        var tokens = _lexer.Tokenize(expression);
        foreach (var token in tokens)
        {
            if (token.TokenType.Name != "Expression")
            {
                throw new InvalidOperationException("Unexpected token type.");
            }

            var parsed = _parser.Parse(token.Text);
            captures.Add(parsed);
        }
        
        return captures.ToArray();
    }

    private static readonly Lazy<LexerExpressionMatcher> Lazy = new(() => new LexerExpressionMatcher());
    public static LexerExpressionMatcher Instance => Lazy.Value;
}