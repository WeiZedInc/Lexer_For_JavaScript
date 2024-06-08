namespace Lexer_For_JavaScript
{
    public enum TokenType
    {
        Identifier,
        Keyword,
        Number,
        String,
        Operator,
        Punctuation,
        Whitespace,
        Comment,
        Error
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public int Position { get; }
        public string ErrorMessage { get; }

        public Token(TokenType type, string value, int position, string errorMessage = null)
        {
            Type = type;
            Value = value;
            Position = position;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return ErrorMessage == null ? $"{Type}: {Value} at {Position}" : $"Error: {ErrorMessage} at {Position}";
        }
    }
}
