using System.Diagnostics.CodeAnalysis;
using System.Text;
using HsManLexer.Exceptions;
using HsManLexer.Readers;
using HsManLexer.Rules;
using HsManLexer.Tokens;

namespace TosuSyncService.Expressions.Lexer;

public static class TosuTokenType
{
    public static TokenType Metadata { get; } = TokenType.Create<string>("Metadata");
}

public class MetadataToken(long position, string k, string v) : Token(position, $"{k}={v}", TosuTokenType.Metadata)
{
    public string Key { get; } = k;
    public string Value { get; } = v;
}

public class MetadataLexerRule : ILexerRule
{
    public string EndIndicator { get; set; } = "}";
    public bool TryParse(SeekableStringReader reader, [NotNullWhen(true)] out Token? token)
    {
        var ch = reader.PeekChar();
        if (ch != '|')
        {
            token = null;
            return false;
        }

        int startPosition = reader.Position;
        reader.Read();
        StringBuilder builder = new StringBuilder();
        string? key = null, value = null;
        bool shouldBreak = false;
        while (!reader.EndOfString || shouldBreak)
        {
            ch = reader.PeekChar();
            switch (ch)
            {
                case '=' when builder.Length == 0:
                    throw new LexerException("Metadata key is missing.");
                case '=':
                    key = builder.ToString();
                    builder.Clear();
                    reader.Read();
                    continue;
                case '"':
                {
                    reader.Read();
                    while (!reader.EndOfString)
                    {
                        ch = reader.PeekChar();
                        if (ch == '\\')
                        {
                            reader.Read();
                            var next = reader.ReadChar();
                            builder.Append(next);
                            continue;
                        }
                        
                        
                        if (ch == '"')
                        {
                            reader.Read();
                            shouldBreak = true;
                            break;
                        }
                    
                        builder.Append(ch);
                        reader.Read();
                    }

                    break;
                }
            }

            if (ch == '|' || ch == ':' || ch == EndIndicator[0] || shouldBreak)
            {
                value = builder.ToString();
                break;
            }
            
            builder.Append(ch);
            reader.Read();
        }

        if (key == null)
        {
            throw new LexerException("Metadata key is missing.");
        }

        value ??= "";
        
        token = new MetadataToken(startPosition, key, value);
        return true;
    }
}