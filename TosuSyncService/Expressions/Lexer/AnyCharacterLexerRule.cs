using System.Diagnostics.CodeAnalysis;
using System.Text;
using FleeExpressionEvaluator;
using HsManLexer.Readers;
using HsManLexer.Rules;
using HsManLexer.Tokens;

namespace TosuSyncService.Expressions.Lexer;

public class AnyCharacterLexerRule : ILexerRule
{
    public HashSet<char> ExceptedChars { get; } = new HashSet<char>();
    public bool TryParse(SeekableStringReader reader, [NotNullWhen(true)] out Token? token)
    {
        var startPosition = reader.Position;
        StringBuilder builder = new StringBuilder();
        while (!reader.EndOfString)
        {
            var ch = reader.PeekChar();
            if (ExceptedChars.Contains(ch))
            {
                break;
            }
            builder.Append(ch);
            reader.Read();
        }
        
        token = new Token(startPosition, builder.ToString(), EvaluatorTokenTypes.Expression);
        return true;
    }
}