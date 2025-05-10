using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public class ShiftState : IParserState
    {
        public void Handle(ParserContext context)
        {
            context.Ss.Add(context.ThisS);
            context.Yystate = context.Action.NextState;
        }
    }

    public class ReduceState : IParserState
    {
        private readonly Formula _formula;
        private readonly int _production;

        public ReduceState(Formula formula, int production)
        {
            _formula = formula;
            _production = production;
        }

        public void Handle(ParserContext context)
        {
            var len = _formula.Productions[_production].Length;
            context.Yy = new Expression();
            context.Ss.RemoveRange(context.Ss.Count - len, len);
            context.ThisS = context.Ss[context.Ss.Count - 1];
            context.Symbol = _formula.Productions[_production].Symbol;
            _formula.ParserPerformAction(context);
        }
    }

    public class AcceptState : IParserState
    {
        public void Handle(ParserContext context)
        {
            context.Yy = context.Ss[1];
            context.Ss.Clear();
        }
    }

    public class ParserStateManager
    {
        private readonly Dictionary<int, IParserState> _states;
        private readonly Formula _formula;

        public ParserStateManager(Formula formula)
        {
            _formula = formula;
            _states = new Dictionary<int, IParserState>
            {
                { 1, new ShiftState() },
                { 2, new ReduceState(formula, 0) },
                { 3, new AcceptState() }
            };
        }

        public void HandleState(ParserContext context)
        {
            if (_states.TryGetValue(context.Action.Type, out var state))
            {
                state.Handle(context);
            }
            else
            {
                throw new ParserException("Invalid parser state", new ParserError());
            }
        }
    }
} 