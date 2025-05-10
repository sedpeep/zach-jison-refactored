using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public class SymbolTable : ISymbolTable
    {
        private readonly Dictionary<string, ParserSymbol> _symbolsByName;
        private readonly Dictionary<int, ParserSymbol> _symbolsByIndex;

        public SymbolTable()
        {
            _symbolsByName = new Dictionary<string, ParserSymbol>();
            _symbolsByIndex = new Dictionary<int, ParserSymbol>();
        }

        public void Add(ParserSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            _symbolsByName[symbol.Name] = symbol;
            _symbolsByIndex[symbol.Index] = symbol;
        }

        public ParserSymbol Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Symbol name cannot be null or empty", nameof(name));
            }

            return _symbolsByName.TryGetValue(name, out var symbol) ? symbol : null;
        }

        public ParserSymbol Get(int index)
        {
            if (index < 0)
            {
                throw new ArgumentException("Symbol index cannot be negative", nameof(index));
            }

            return _symbolsByIndex.TryGetValue(index, out var symbol) ? symbol : null;
        }

        public bool Contains(string name)
        {
            return _symbolsByName.ContainsKey(name);
        }

        public bool Contains(int index)
        {
            return _symbolsByIndex.ContainsKey(index);
        }

        public IEnumerable<ParserSymbol> GetAllSymbols()
        {
            return _symbolsByIndex.Values;
        }
    }
} 