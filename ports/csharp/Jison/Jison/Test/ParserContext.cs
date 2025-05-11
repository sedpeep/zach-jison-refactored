using System;

namespace jQuerySheet
{
    public class ParserContext
    {
        public JList<Expression> Ss { get; set; }
        public int Yystate { get; set; }
        public Expression Yy { get; set; }
        public Expression ThisS { get; set; }
        public ParserSymbol Symbol { get; set; }
        public IParserAction Action { get; set; }
        public ParserLocation Location { get; set; }

        public ParserContext()
        {
            Ss = new JList<Expression>();
            Location = new ParserLocation();
        }

        public void PushState(Expression state)
        {
            Ss.Add(state);
            Yystate = Ss.Count - 1;
        }

        public Expression PopState()
        {
            if (Ss.Count == 0)
            {
                throw new InvalidOperationException("Cannot pop from empty state stack");
            }

            var state = Ss[Ss.Count - 1];
            Ss.RemoveAt(Ss.Count - 1);
            Yystate = Ss.Count - 1;
            return state;
        }

        public void UpdateLocation(int line, int column)
        {
            Location.Line = line;
            Location.Column = column;
        }

        public void Clear()
        {
            Ss.Clear();
            Yystate = 0;
            Yy = null;
            ThisS = null;
            Symbol = null;
            Action = null;
            Location = new ParserLocation();
        }
    }

    public class ParserLocation
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public ParserLocation()
        {
            Line = 1;
            Column = 0;
        }

        public override string ToString()
        {
            return $"Line {Line}, Column {Column}";
        }
    }
} 