using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public static class ParserProductionFactory
    {
        private static readonly Dictionary<int, Production> _productions = new Dictionary<int, Production>();
        private static int _nextIndex = 0;

        public static Production CreateProduction(ParserSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var production = new Production(symbol, _nextIndex++);
            _productions[production.Index] = production;
            return production;
        }

        public static Production CreateProduction(ParserSymbol symbol, List<ParserSymbol> symbols)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }
            if (symbols == null)
            {
                throw new ArgumentNullException(nameof(symbols));
            }

            var production = new Production(symbol, symbols, _nextIndex++);
            _productions[production.Index] = production;
            return production;
        }

        public static Production GetProduction(int index)
        {
            if (!_productions.TryGetValue(index, out var production))
            {
                throw new ArgumentException($"Production with index {index} does not exist");
            }
            return production;
        }

        public static void AddSymbol(int productionIndex, ParserSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var production = GetProduction(productionIndex);
            production.AddSymbol(symbol);
        }

        public static void Clear()
        {
            _productions.Clear();
            _nextIndex = 0;
        }
    }
} 