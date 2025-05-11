using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public class Production
    {
        public ParserSymbol Symbol { get; }
        public List<ParserSymbol> Symbols { get; }
        public int Length => Symbols.Count;
        public int Index { get; }

        public Production(ParserSymbol symbol, List<ParserSymbol> symbols, int index)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
            Index = index;
        }

        public Production(ParserSymbol symbol, int index) : this(symbol, new List<ParserSymbol>(), index)
        {
        }

        public void AddSymbol(ParserSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }
            Symbols.Add(symbol);
        }

        public override string ToString()
        {
            return $"{Symbol} -> {string.Join(" ", Symbols)}";
        }
    }
} 