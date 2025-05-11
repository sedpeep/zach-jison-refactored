using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public static class ParserStateFactory
    {
        private static readonly Dictionary<int, ParserState> _states = new Dictionary<int, ParserState>();

        public static ParserState CreateState(int index)
        {
            if (!_states.TryGetValue(index, out var state))
            {
                state = new ParserState(index);
                _states[index] = state;
            }
            return state;
        }

        public static ParserState GetState(int index)
        {
            if (!_states.TryGetValue(index, out var state))
            {
                throw new ArgumentException($"State {index} does not exist");
            }
            return state;
        }

        public static void AddTransition(int stateIndex, ParserSymbol symbol, StateTransition transition)
        {
            var state = GetState(stateIndex);
            state.AddTransition(symbol, transition);
        }

        public static void AddProduction(int stateIndex, Production production)
        {
            var state = GetState(stateIndex);
            state.AddProduction(production);
        }

        public static void Clear()
        {
            _states.Clear();
        }
    }
} 