using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public class ParserState
    {
        public int Index { get; }
        public Dictionary<int, StateTransition> Transitions { get; }
        public List<Production> Productions { get; }

        public ParserState(int index)
        {
            Index = index;
            Transitions = new Dictionary<int, StateTransition>();
            Productions = new List<Production>();
        }

        public void AddTransition(ParserSymbol symbol, StateTransition transition)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }
            if (transition == null)
            {
                throw new ArgumentNullException(nameof(transition));
            }

            Transitions[symbol.Index] = transition;
        }

        public void AddProduction(Production production)
        {
            if (production == null)
            {
                throw new ArgumentNullException(nameof(production));
            }

            Productions.Add(production);
        }

        public StateTransition GetTransition(ParserSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            return Transitions.TryGetValue(symbol.Index, out var transition) ? transition : null;
        }

        public override string ToString()
        {
            return $"State {Index}";
        }
    }
} 