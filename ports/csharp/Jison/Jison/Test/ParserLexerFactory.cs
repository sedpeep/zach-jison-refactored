using System;

namespace jQuerySheet
{
    public static class ParserLexerFactory
    {
        public static ILexer CreateLexer()
        {
            return new FormulaLexer();
        }

        public static ILexer CreateLexer(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be null or empty", nameof(input));
            }

            var lexer = new FormulaLexer();
            lexer.SetInput(input);
            return lexer;
        }

        public static ILexer CreateLexer(ILexer template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            var lexer = new FormulaLexer();
            if (template is FormulaLexer formulaLexer)
            {
                // Copy state from template if needed
            }
            return lexer;
        }
    }
} 