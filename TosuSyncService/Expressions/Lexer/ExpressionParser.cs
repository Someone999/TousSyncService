using FleeExpressionEvaluator.Expressions;
using FleeExpressionEvaluator.Lexers.Rules;
using HsManLexer.Lexers;
using HsManLexer.Rules;
using HsManLexer.Tokens;

namespace TosuSyncService.Expressions.Lexer;

public class ExpressionParser
{
    private RuleBasedLexer _lexer = new RuleBasedLexer();
    private ExpressionParser()
    {
        var listMatchRule = new ListMatchLexerRule(TokenTypes.Symbol);
        listMatchRule.AddMatchString("|");
        listMatchRule.AddMatchString("=");
        listMatchRule.AddMatchString("${");
        listMatchRule.AddMatchString("}");
        _lexer.Rules.Add(listMatchRule);
        var anyCharacterRule = new AnyCharacterLexerRule();
        anyCharacterRule.ExceptedChars.Add('|');
        anyCharacterRule.ExceptedChars.Add(':');
        anyCharacterRule.ExceptedChars.Add('=');
        anyCharacterRule.ExceptedChars.Add('$');
        anyCharacterRule.ExceptedChars.Add('{');
        anyCharacterRule.ExceptedChars.Add('}');
        _lexer.Rules.Add(anyCharacterRule);
        _lexer.Rules.Add(new FormatLexerRule());
        _lexer.Rules.Add(new MetadataLexerRule());
    }

    private KeyValuePair<string, string> ReadNextMetadata(TokenStream tokenStream)
    {
        tokenStream.MoveNext();
        var current = tokenStream.Current;

        if (current?.TokenType.Name != "Expression")
        {
            throw new Exception("Unexpected token type");
        }
        
        var equalIdx = current.Text.IndexOf('=');
        var key = current.Text[..equalIdx];
        var value = current.Text[(equalIdx + 1)..];
        return new KeyValuePair<string, string>(key, value);
    }
    
    public IExpression Parse(string expression)
    {
        var tokenStream = new TokenStream(_lexer.Tokenize(expression));
        if (!tokenStream.MoveNext())
        {
            throw new InvalidOperationException("Unexpected end of expression");
        }
        
        
        tokenStream.MatchOrThrow(t => t is { TokenType.Name: "Symbol" }, "Invalid expression");
        tokenStream.MoveNext();

        var exp = tokenStream.Current;
        if (exp == null)
        {
            throw new InvalidOperationException("Unexpected end of expression");
        }
        
        TosuExpression capture = new TosuExpression(expression, exp.Text);
        while (tokenStream.MoveNext())
        {
            var current = tokenStream.Current;
            if (current == null)
            {
                throw new InvalidOperationException("Unexpected end of expression");
            }

            switch (current.TokenType.Name)
            {
                case "Format":
                    capture.OutputFormat = current.Text;
                    continue;
                case "Metadata" when current is MetadataToken token:
                    capture.Metadata.TryAdd(token.Key, token.Value);
                    break;
                case "Symbol" when current.Text is "}":
                    return capture;
            }
        }

        return capture;
    }

    private static readonly Lazy<ExpressionParser> Lazy = new(() => new ExpressionParser());
    public static ExpressionParser Instance => Lazy.Value;
}