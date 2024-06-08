namespace Lexer_For_JavaScript
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string code = "varible x = 10;\nif (x > 5) {\n  console.log(\"x is greater than 5\");\n}";
            Lexer lexer = new Lexer(code);
            List<Token> tokens = lexer.Tokenize();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
}
