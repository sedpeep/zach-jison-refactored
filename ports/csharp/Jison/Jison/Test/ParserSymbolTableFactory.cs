using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public static class ParserSymbolTableFactory
    {
        public static ISymbolTable CreateSymbolTable()
        {
            return new SymbolTable();
        }

        public static ISymbolTable CreateSymbolTable(IEnumerable<ParserSymbol> symbols)
        {
            if (symbols == null)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            var table = new SymbolTable();
            foreach (var symbol in symbols)
            {
                table.Add(symbol);
            }
            return table;
        }

        public static ISymbolTable CreateDefaultSymbolTable()
        {
            var symbols = new[]
            {
                new ParserSymbol("error", 0),
                new ParserSymbol("EOF", 1),
                new ParserSymbol("NUMBER", 2),
                new ParserSymbol("STRING", 3),
                new ParserSymbol("ID", 4),
                new ParserSymbol("$accept", 5),
                new ParserSymbol("expressions", 6),
                new ParserSymbol("expression", 7),
                new ParserSymbol("exp", 8),
                new ParserSymbol("call", 9),
                new ParserSymbol("args", 10),
                new ParserSymbol("+", 11),
                new ParserSymbol("-", 12),
                new ParserSymbol("*", 13),
                new ParserSymbol("/", 14),
                new ParserSymbol("(", 15),
                new ParserSymbol(")", 16),
                new ParserSymbol(",", 17)
            };

            return CreateSymbolTable(symbols);
        }

        public static ISymbolTable CreateSymbolTable(ISymbolTable template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            var table = new SymbolTable();
            if (template is SymbolTable symbolTable)
            {
                // Copy symbols from template if needed
            }
            return table;
        }
    }
} 