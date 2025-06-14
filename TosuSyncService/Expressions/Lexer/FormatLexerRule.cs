using System.Diagnostics.CodeAnalysis;
using System.Text;
using HsManLexer.Readers;
using HsManLexer.Rules;
using HsManLexer.Tokens;

namespace TosuSyncService.Expressions.Lexer;

public class FormatLexerRule : ILexerRule
{
    public string EndIndicator { get; set; } = "}";
    public bool TryParse(SeekableStringReader reader, [NotNullWhen(true)] out Token? token)
    {
        var colon = reader.PeekChar();
        if (colon != ':')
        {
            token = null;
            return false;
        }

        reader.Read();
        var startPos = reader.Position;
        StringBuilder builder = new StringBuilder();
        while (!reader.EndOfString)
        {
            var ch = reader.PeekChar();
            if (ch == '|') // Metadata
            {
                break;
            }
            
            if (ch != EndIndicator[0])
            {
                builder.Append(ch);
                reader.Read();
                continue;
            }

            var endIndicator = reader.PeekString(EndIndicator.Length);
            if (endIndicator == EndIndicator)
            {
                break;
            }
            
            builder.Append(endIndicator);
            reader.ConsumeChars(endIndicator.Length);
        }
        
        token = new Token(startPos, builder.ToString(), TokenType.Create<string>("Format"));
        return true;
    }
}