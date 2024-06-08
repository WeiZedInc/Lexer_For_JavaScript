namespace Lexer_For_JavaScript
{
    public class Lexer
    {
        private string input;
        private int position;
        private int length;

        private static readonly string[] Keywords = { "var", "let", "const", "if", "else", "for", "while", "function", "return" };
        private static readonly string[] Operators = { "+", "-", "*", "/", "=", "==", "===", "!=", "!==", "<", ">", "<=", ">=", "&&", "||", "!", "++", "--" };

        public Lexer(string input)
        {
            this.input = input;
            position = 0;
            length = input.Length;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (position < length)
            {
                char current = input[position];

                if (char.IsWhiteSpace(current))
                {
                    tokens.Add(ScanWhitespace());
                }
                else if (char.IsLetter(current) || current == '_')
                {
                    tokens.Add(ScanIdentifierOrKeyword());
                }
                else if (char.IsDigit(current))
                {
                    tokens.Add(ScanNumber());
                }
                else if (current == '"' || current == '\'')
                {
                    tokens.Add(ScanString());
                }
                else if (current == '/' && (Peek() == '/' || Peek() == '*'))
                {
                    tokens.Add(ScanComment());
                }
                else if (IsOperatorStart(current))
                {
                    tokens.Add(ScanOperator());
                }
                else if (IsPunctuation(current))
                {
                    tokens.Add(new Token(TokenType.Punctuation, current.ToString(), position));
                    position++;
                }
                else
                {
                    tokens.Add(new Token(TokenType.Error, current.ToString(), position, "Unexpected character"));
                    position++;
                }
            }

            return tokens;
        }

        private Token ScanWhitespace()
        {
            int start = position;
            while (position < length && char.IsWhiteSpace(input[position]))
            {
                position++;
            }

            string value = input.Substring(start, position - start);
            return new Token(TokenType.Whitespace, value, start);
        }

        private Token ScanIdentifierOrKeyword()
        {
            int start = position;
            while (position < length && (char.IsLetterOrDigit(input[position]) || input[position] == '_'))
            {
                position++;
            }

            string value = input.Substring(start, position - start);
            if (Array.Exists(Keywords, keyword => keyword == value))
            {
                return new Token(TokenType.Keyword, value, start);
            }
            else if (value == "varible") // Specific check for common typo
            {
                return new Token(TokenType.Error, value, start, "Unknown keyword 'varible'. Did you mean 'var'?");
            }
            else if (value == "funtion") // Specific check for common typo
            {
                return new Token(TokenType.Error, value, start, "Unknown keyword 'funtion'. Did you mean 'function'?");
            }
            else
            {
                return new Token(TokenType.Identifier, value, start);
            }
        }

        private Token ScanNumber()
        {
            int start = position;
            var state = 0; // Initial state
            while (position < length)
            {
                char current = input[position];
                switch (state)
                {
                    case 0:
                        if (char.IsDigit(current))
                        {
                            state = 1;
                            position++;
                        }
                        else
                        {
                            return new Token(TokenType.Error, input.Substring(start, position - start), start, "Invalid number format");
                        }
                        break;
                    case 1:
                        if (char.IsDigit(current))
                        {
                            position++;
                        }
                        else if (current == '.')
                        {
                            state = 2;
                            position++;
                        }
                        else
                        {
                            return new Token(TokenType.Number, input.Substring(start, position - start), start);
                        }
                        break;
                    case 2:
                        if (char.IsDigit(current))
                        {
                            state = 3;
                            position++;
                        }
                        else
                        {
                            return new Token(TokenType.Error, input.Substring(start, position - start), start, "Invalid number format");
                        }
                        break;
                    case 3:
                        if (char.IsDigit(current))
                        {
                            position++;
                        }
                        else
                        {
                            return new Token(TokenType.Number, input.Substring(start, position - start), start);
                        }
                        break;
                }
            }
            return new Token(TokenType.Number, input.Substring(start, position - start), start);
        }

        private Token ScanString()
        {
            char quoteType = input[position];
            int start = position;
            var state = 0; // Initial state
            position++;
            while (position < length)
            {
                char current = input[position];
                switch (state)
                {
                    case 0:
                        if (current == quoteType)
                        {
                            position++;
                            return new Token(TokenType.String, input.Substring(start, position - start), start);
                        }
                        else if (current == '\\')
                        {
                            state = 1;
                            position++;
                        }
                        else
                        {
                            position++;
                        }
                        break;
                    case 1:
                        position++;
                        state = 0;
                        break;
                }
            }
            return new Token(TokenType.Error, input.Substring(start, position - start), start, "Unterminated string literal");
        }

        private Token ScanComment()
        {
            int start = position;
            position++;
            if (input[position] == '/')
            {
                // Single line comment
                while (position < length && input[position] != '\n')
                {
                    position++;
                }
                return new Token(TokenType.Comment, input.Substring(start, position - start), start);
            }
            else if (input[position] == '*')
            {
                // Multi-line comment
                position++;
                while (position < length)
                {
                    if (input[position] == '*' && Peek() == '/')
                    {
                        position += 2;
                        return new Token(TokenType.Comment, input.Substring(start, position - start), start);
                    }
                    position++;
                }
                return new Token(TokenType.Error, input.Substring(start, position - start), start, "Unterminated comment");
            }
            return new Token(TokenType.Error, input.Substring(start, position - start), start, "Invalid comment");
        }

        private Token ScanOperator()
        {
            int start = position;
            var state = 0; // Initial state
            while (position < length)
            {
                char current = input[position];
                switch (state)
                {
                    case 0:
                        if (IsOperatorStart(current))
                        {
                            state = 1;
                            position++;
                        }
                        else
                        {
                            return new Token(TokenType.Error, input.Substring(start, position - start), start, "Invalid operator");
                        }
                        break;
                    case 1:
                        if (IsOperator(input.Substring(start, position - start + 1)))
                        {
                            position++;
                        }
                        else
                        {
                            return new Token(TokenType.Operator, input.Substring(start, position - start), start);
                        }
                        break;
                }
            }
            return new Token(TokenType.Operator, input.Substring(start, position - start), start);
        }

        private bool IsOperatorStart(char ch)
        {
            return Array.Exists(Operators, op => op.StartsWith(ch.ToString()));
        }

        private bool IsOperator(string value)
        {
            return Array.Exists(Operators, op => op == value);
        }

        private bool IsPunctuation(char ch)
        {
            return ch == ';' || ch == ',' || ch == '.' || ch == '(' || ch == ')' || ch == '{' || ch == '}' || ch == '[' || ch == ']';
        }

        private char Peek()
        {
            return position + 1 < length ? input[position + 1] : '\0';
        }
    }
}