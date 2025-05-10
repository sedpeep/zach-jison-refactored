using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public enum ParserActionType
    {
        None = 0,
        Shift = 1,
        Reduce = 2,
        Accept = 3
    }

    public interface ISymbolTable
    {
        void Add(ParserSymbol symbol);
        ParserSymbol Get(string name);
        ParserSymbol Get(int index);
        bool Contains(string name);
        bool Contains(int index);
        IEnumerable<ParserSymbol> GetAllSymbols();
    }

    public interface IParserState
    {
        void Handle(ParserContext context);
    }

    public class ParserContext
    {
        public Expression ThisS { get; set; }
        public Expression Yy { get; set; }
        public int Yystate { get; set; }
        public JList<Expression> Ss { get; set; }
        public ParserSymbol Symbol { get; set; }
        public ParserAction Action { get; set; }
    }

    public class ParserException : Exception
    {
        public ParserError Error { get; }

        public ParserException(string message, ParserError error) : base(message)
        {
            Error = error;
        }
    }

    public class NullParserValue : ParserValue
    {
        public static readonly NullParserValue Instance = new NullParserValue();
        private NullParserValue() { }
    }

    public class StateTransition
    {
        public int CurrentState { get; }
        public int NextState { get; }
        public ParserAction Action { get; }

        public StateTransition(int currentState, int nextState, ParserAction action)
        {
            CurrentState = currentState;
            NextState = nextState;
            Action = action;
        }
    }
} 