using System;

namespace jQuerySheet
{
    public class StateTransition
    {
        public IParserAction Action { get; }
        public int NextState { get; }
        public ParserSymbol Symbol { get; }

        public StateTransition(IParserAction action, int nextState, ParserSymbol symbol)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            NextState = nextState;
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }

        public void Execute(ParserContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Action.Execute(context);
            context.Yystate = NextState;
        }

        public override string ToString()
        {
            return $"StateTransition: {Symbol} -> {NextState}";
        }
    }
} 