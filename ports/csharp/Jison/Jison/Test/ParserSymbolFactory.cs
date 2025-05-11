using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public static class ParserSymbolFactory
    {
        private static readonly Dictionary<string, ParserSymbol> _symbolsByName = new Dictionary<string, ParserSymbol>();
        private static readonly Dictionary<int, ParserSymbol> _symbolsByIndex = new Dictionary<int, ParserSymbol>();
        private static int _nextIndex = 0;

        public static ParserSymbol CreateSymbol(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Symbol name cannot be null or empty", nameof(name));
            }

            if (!_symbolsByName.TryGetValue(name, out var symbol))
            {
                symbol = new ParserSymbol(name, _nextIndex++);
                _symbolsByName[name] = symbol;
                _symbolsByIndex[symbol.Index] = symbol;
            }
            return symbol;
        }

        public static ParserSymbol CreateSymbol(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Symbol name cannot be null or empty", nameof(name));
            }

            var symbol = new ParserSymbol(name, _nextIndex++, value);
            _symbolsByName[name] = symbol;
            _symbolsByIndex[symbol.Index] = symbol;
            return symbol;
        }

        public static ParserSymbol GetSymbol(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Symbol name cannot be null or empty", nameof(name));
            }

            if (!_symbolsByName.TryGetValue(name, out var symbol))
            {
                throw new ArgumentException($"Symbol '{name}' does not exist");
            }
            return symbol;
        }

        public static ParserSymbol GetSymbol(int index)
        {
            if (!_symbolsByIndex.TryGetValue(index, out var symbol))
            {
                throw new ArgumentException($"Symbol with index {index} does not exist");
            }
            return symbol;
        }

        public static void Clear()
        {
            _symbolsByName.Clear();
            _symbolsByIndex.Clear();
            _nextIndex = 0;
        }
    }
} 