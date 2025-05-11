using System;

namespace jQuerySheet
{
    public static class ParserStateTransitionFactory
    {
        public static StateTransition CreateTransition(IParserAction action, int nextState, ParserSymbol symbol)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            return new StateTransition(action, nextState, symbol);
        }

        public static StateTransition CreateShiftTransition(int nextState, ParserSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var action = ParserActionFactory.CreateShiftAction(nextState);
            return new StateTransition(action, nextState, symbol);
        }

        public static StateTransition CreateReduceTransition(int production, ParserSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var action = ParserActionFactory.CreateReduceAction(production);
            return new StateTransition(action, production, symbol);
        }

        public static StateTransition CreateAcceptTransition(ParserSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var action = ParserActionFactory.CreateAcceptAction();
            return new StateTransition(action, 0, symbol);
        }
    }
} 