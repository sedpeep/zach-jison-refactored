using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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

	public class Formula
	{
		private readonly SymbolTable _symbolTable;
		private readonly ParserStateManager _stateManager;
		private readonly Dictionary<int, Dictionary<int, StateTransition>> _transitions;
		public JList<ParserSymbol> Symbols { get; private set; }
		public Dictionary<int, ParserSymbol> Terminals { get; private set; }
		public JList<ParserProduction> Productions { get; private set; }
		public JList<JList<ParserAction>> Table { get; private set; }

		public Formula()
		{
			_symbolTable = new SymbolTable();
			_stateManager = new ParserStateManager(this);
			_transitions = new Dictionary<int, Dictionary<int, StateTransition>>();
			Symbols = new JList<ParserSymbol>();
			Terminals = new Dictionary<int, ParserSymbol>();
			Productions = new JList<ParserProduction>();
			Table = new JList<JList<ParserAction>>();

			InitializeSymbols();
			InitializeTransitions();
		}

		private void InitializeSymbols()
		{
			var symbols = new[]
			{
				new ParserSymbol("error", 0),
				new ParserSymbol("EOF", 1),
				new ParserSymbol("NUMBER", 2),
				new ParserSymbol("STRING", 3),
				new ParserSymbol("ID", 4),
				new ParserSymbol("$accept", 5),
				new ParserSymbol("expressions", 6),
				new ParserSymbol("expression", 7),
				new ParserSymbol("exp", 8),
				new ParserSymbol("call", 9),
				new ParserSymbol("args", 10),
				new ParserSymbol("+", 11),
				new ParserSymbol("-", 12),
				new ParserSymbol("*", 13),
				new ParserSymbol("/", 14),
				new ParserSymbol("(", 15),
				new ParserSymbol(")", 16),
				new ParserSymbol(",", 17)
			};

			foreach (var symbol in symbols)
			{
				_symbolTable.Add(symbol);
				Symbols.Add(symbol);
			}
		}

		private void InitializeTransitions()
		{
			// Initialize state transitions
			// This would be populated with actual transition data
		}

		public Expression Parse(string input)
		{
			var context = new ParserContext
			{
				Ss = new JList<Expression>(),
				Yystate = 0
			};

			try
			{
				while (true)
				{
					var action = GetNextAction(context);
					if (action == null)
					{
						throw new ParserException("Unexpected end of input", new ParserError());
					}

					context.Action = action;
					_stateManager.HandleState(context);

					if (action.Type == (int)ParserActionType.Accept)
					{
						return context.Yy;
					}
				}
			}
			catch (ParserException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ParserException("Parser error", new ParserError { Message = ex.Message });
			}
		}

		private ParserAction GetNextAction(ParserContext context)
		{
			if (!_transitions.TryGetValue(context.Yystate, out var stateTransitions))
			{
				return null;
			}

			var symbol = GetNextSymbol();
			if (symbol == null)
			{
				return null;
			}

			if (stateTransitions.TryGetValue(symbol.Index, out var transition))
			{
				return transition.Action;
			}

			return null;
		}

		private ParserSymbol GetNextSymbol()
		{
			// Implementation for getting next symbol from input
			return null;
		}

		public void ParserPerformAction(ParserContext context)
		{
			// Implementation of parser actions
		}

		public void Trace()
		{

		}

		public void ParseError(string error, ParserError hash = null)
		{
			throw new InvalidOperationException(error);
		}

		public void LexerError(string error, LexerError hash = null)
		{
			throw new InvalidOperationException(error);
		}

		public Expression Yy = new Expression();
		public string Match = "";
		public string Matched = "";
		public Stack<string> ConditionStack;
		public Dictionary<int, Regex> Rules;
		public Dictionary<string, LexerConditions> Conditions;
		public bool Done = false;
		public bool Less;
		public bool _More;
		public string _Input;
		public int Offset;
		public Dictionary<int, ParserRange>Ranges;
		public bool Flex = false;

		public void SetInput(string input)
		{
			_Input = input;
			_More = Less = Done = false;
			Yy.LineNo = Yy.Leng = 0;
			Matched = Match = "";
			ConditionStack = new Stack<string>();
			ConditionStack.Push("INITIAL");

			if (Ranges != null)
			{
				Yy.Loc = new ParserLocation(new ParserRange(0,0));
			} else {
				Yy.Loc = new ParserLocation();
			}

			Offset = 0;
		}

		public string Input()
		{
			string ch = _Input[0].ToString();
			Yy.Text += ch;
			Yy.Leng++;
			Offset++;
			Match += ch;
			Matched += ch;
			Match lines = Regex.Match(ch, "/(?:\r\n?|\n).*/");
			if (lines.Success) {
				Yy.LineNo++;
				Yy.Loc.LastLine++;
			} else {
				Yy.Loc.LastColumn++;
			}

			if (Ranges != null)
			{
				Yy.Loc.Range.Y++;
			}

			_Input = _Input.Substring(1);
			return ch;
		}

		public void Unput(string ch)
		{
			int len = ch.Length;
			var lines = Regex.Split(ch, "/(?:\r\n?|\n)/");

			_Input = ch + _Input;
			Yy.Text = Yy.Text.Substring(0, len - 1);
			Offset -= len;
			var oldLines = Regex.Split(Match, "/(?:\r\n?|\n)/");
			Match = Match.Substring(0, Match.Length - 1);
			Matched = Matched.Substring(0, Matched.Length - 1);

			if ((lines.Length - 1) > 0) Yy.LineNo -= lines.Length - 1;
			var r = Yy.Loc.Range;

			Yy.Loc = new ParserLocation(
				Yy.Loc.FirstLine,
				Yy.LineNo + 1,
				Yy.Loc.FirstColumn,
				(
					lines.Length > 0 ?
					(
						lines.Length == oldLines.Length ?
							Yy.Loc.FirstColumn :
							0
					) + oldLines[oldLines.Length - lines.Length].Length - lines[0].Length :
					Yy.Loc.FirstColumn - len
				)
			);

			if (Ranges.Count > 0) {
				Yy.Loc.Range = new ParserRange(r.X, r.X + Yy.Leng - len);
			}
		}

		public void More()
		{
			_More = true;
		}

		public string PastInput()
		{
			var past = Matched.Substring(0, Matched.Length - Match.Length);
			return (past.Length > 20 ? "..." + Regex.Replace(past.Substring(-20), "/\n/", "") : "");
		}

		public string UpcomingInput()
		{
			var next = Match;
			if (next.Length < 20)
			{
				next += _Input.Substring(0, (next.Length > 20 ? 20 - next.Length : next.Length));
			}
			return Regex.Replace(next.Substring(0, (next.Length > 20 ? 20 - next.Length : next.Length)) + (next.Length > 20 ? "..." : ""), "/\n/", "");
		}

		public string ShowPosition()
		{
			var pre = PastInput();

			var c = "";
			for (var i = 0; i < pre.Length; i++)
			{
				c += "-";
			}

			return pre + UpcomingInput() + '\n' + c + "^";
		}

		public ParserSymbol Next()
		{
			if (Done == true)
			{
				return Symbols["EOF"];
			}

			if (String.IsNullOrEmpty(_Input))
			{
				Done = true;
			}

			if (_More == false)
			{
				Yy.Text = "";
				Match = "";
			}

			var rules = CurrentRules();
			string match = "";
			bool matched = false;
			int index = 0;
			Regex rule;
			for (int i = 0; i < rules.Count; i++)
			{
				rule = Rules[rules[i]];
				var tempMatch = rule.Match(_Input);
				if (tempMatch.Success == true && (match != null || tempMatch.Length > match.Length)) {
					match = tempMatch.Value;
					matched = true;
					index = i;
					if (!Flex) {
						break;
					}
				}
			}
			if ( matched )
			{
				Match lineCount = Regex.Match(match, "/\n.*/");

				Yy.LineNo += lineCount.Length;
				Yy.Loc.FirstLine = Yy.Loc.LastLine;
				Yy.Loc.LastLine = Yy.LineNo + 1;
				Yy.Loc.FirstColumn = Yy.Loc.LastColumn;
				Yy.Loc.LastColumn = lineCount.Length > 0 ? lineCount.Length - 1 : Yy.Loc.LastColumn + match.Length;

				Yy.Text += match;
				Match += match;
				Matched += match;

				Yy.Leng = Yy.Text.Length;
				if (Ranges != null)
				{
					Yy.Loc.Range = new ParserRange(Offset, Offset += Yy.Leng);
				}
				_More = false;
				_Input = _Input.Substring(match.Length);
                var ruleIndex = rules[index];
                var nextCondition = ConditionStack.Peek();
                dynamic action = LexerPerformAction(ruleIndex, nextCondition);
				ParserSymbol token = Symbols[action];

				if (Done == true && String.IsNullOrEmpty(_Input) == false)
				{
					Done = false;
				}

				if (token.Index > -1) {
					return token;
				} else {
					return null;
				}
			}

			if (String.IsNullOrEmpty(_Input)) {
				return Symbols["EOF"];
			} else
			{
				LexerError("Lexical error on line " + (Yy.LineNo + 1) + ". Unrecognized text.\n" + ShowPosition(), new LexerError("", -1, Yy.LineNo));
				return null;
			}
		}

		public ParserSymbol LexerLex()
		{
			var r = Next();

			while (r == null)
			{
			    r = Next();
			}

		    return r;
		}

		public void Begin(string condition)
		{
			ConditionStack.Push(condition);
		}

		public string PopState()
		{
			return ConditionStack.Pop();
		}

		public List<int> CurrentRules()
		{
			var peek = ConditionStack.Peek();
			return Conditions[peek].Rules;
		}

		public dynamic LexerPerformAction(int avoidingNameCollisions, string Yy_Start)
		{
			

;
switch(avoidingNameCollisions) {
case 0:/* skip whitespace */
break;
case 1:return 10;
break;
case 2:return 10;
break;
case 3:return 23;
break;
case 4:return 7;
break;
case 5:return 8;
break;
case 6:
	
	
	
		return 29;
		//return 33;
	

break;
case 7:
	
	
	
		return 26;
		//return 33;
	

break;
case 8:
	
	
	
		return 28;
		//return 33;
	

break;
case 9:return 23;
break;
case 10:return 33;
break;
case 11:return 33;
break;
case 12:return 35;
break;
case 13:/* skip whitespace */
break;
case 14:return 34;
break;
case 15:return 27;
break;
case 16:return 31;
break;
case 17:return 32;
break;
case 18:return 19;
break;
case 19:return 20;
break;
case 20:return 18;
break;
case 21:return 12;
break;
case 22:return 21;
break;
case 23:return 13;
break;
case 24:return 14;
break;
case 25:return 16;
break;
case 26:return 15;
break;
case 27:return 17;
break;
case 28:return 22;
break;
case 29:return '"';
break;
case 30:return "'";
break;
case 31:return "!";
break;
case 32:return 11;
break;
case 33:return 36;
break;
case 34:return 37;
break;
case 35:return 5;
break;
}

			return -1;
		}
	}

	public class ParserLocation
	{
		public int FirstLine = 1;
		public int LastLine = 0;
		public int FirstColumn = 1;
		public int LastColumn = 0;
		public ParserRange Range;

		public ParserLocation()
		{
		}

		public ParserLocation(ParserRange range)
		{
			Range = range;
		}

		public ParserLocation(int firstLine, int lastLine, int firstColumn, int lastColumn)
		{
			FirstLine = firstLine;
			LastLine = lastLine;
			FirstColumn = firstColumn;
			LastColumn = lastColumn;
		}

		public ParserLocation(int firstLine, int lastLine, int firstColumn, int lastColumn, ParserRange range)
		{
			FirstLine = firstLine;
			LastLine = lastLine;
			FirstColumn = firstColumn;
			LastColumn = lastColumn;
			Range = range;
		}
	}

	public class LexerConditions
	{
		public List<int> Rules;
		public bool Inclusive;

		public LexerConditions(List<int> rules, bool inclusive)
		{
			Rules = rules;
			Inclusive = inclusive;
		}
	}

	public class ParserProduction
	{
		public int Len = 0;
		public ParserSymbol Symbol;

		public ParserProduction(ParserSymbol symbol)
		{
			Symbol = symbol;
		}

		public ParserProduction(ParserSymbol symbol, int len)
		{
			Symbol = symbol;
			Len = len;
		}
	}

	public class ParserCachedAction
	{
		public ParserAction Action;
		public ParserSymbol Symbol;

		public ParserCachedAction(ParserAction action)
		{
			Action = action;
		}

		public ParserCachedAction(ParserAction action, ParserSymbol symbol)
		{
			Action = action;
			Symbol = symbol;
		}
	}

	public class ParserAction
	{
		public int Action;
		public ParserState State;
		public ParserSymbol Symbol;

		public ParserAction(int action)
		{
			Action = action;
		}

		public ParserAction(int action, ref ParserState state)
		{
			Action = action;
			State = state;
		}

		public ParserAction(int action, ParserState state)
		{
			Action = action;
			State = state;
		}

		public ParserAction(int action, ref ParserSymbol symbol)
		{
			Action = action;
			Symbol = symbol;
		}
	}

	public class ParserSymbol
	{
		public string Name;
		public int Index = -1;
		public IDictionary<int, ParserSymbol> Symbols = new Dictionary<int, ParserSymbol>();
		public IDictionary<string, ParserSymbol> SymbolsByName = new Dictionary<string, ParserSymbol>();

		public ParserSymbol()
		{
		}

		public ParserSymbol(string name, int index)
		{
			Name = name;
			Index = index;
		}

		public void AddAction(ParserSymbol p)
		{
			Symbols.Add(p.Index, p);
			SymbolsByName.Add(p.Name, p);
		}
	}

	public class ParserError
	{
		public String Text;
		public ParserState State;
		public ParserSymbol Symbol;
		public int LineNo;
		public ParserLocation Loc;
		public Stack<string> Expected;

		public ParserError(String text, ParserState state, ParserSymbol symbol, int lineNo, ParserLocation loc, Stack<string> expected)
		{
			Text = text;
			State = state;
			Symbol = symbol;
			LineNo = lineNo;
			Loc = loc;
			Expected = expected;
		}
	}

	public class LexerError
	{
		public String Text;
		public int Token;
		public int LineNo;

		public LexerError(String text, int token, int lineNo)
		{
			Text = text;
			Token = token;
			LineNo = lineNo;
		}
	}

	public class ParserState
	{
		public int Index;
		public Dictionary<int, ParserAction> Actions = new Dictionary<int, ParserAction>();

		public ParserState(int index)
		{
			Index = index;
		}

		public void SetActions(ref Dictionary<int, ParserAction> actions)
		{
			Actions = actions;
		}
	}

	public class ParserRange
	{
		public int X;
		public int Y;

		public ParserRange(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public class ParserSymbols
	{
		private Dictionary<string, ParserSymbol> SymbolsString = new Dictionary<string, ParserSymbol>();
		private Dictionary<int, ParserSymbol> SymbolsInt = new Dictionary<int, ParserSymbol>();

		public void Add(ParserSymbol symbol)
		{
			SymbolsInt.Add(symbol.Index, symbol);
			SymbolsString.Add(symbol.Name, symbol);
		}

		public ParserSymbol this[char name]
		{
			get
			{
				return SymbolsString[name.ToString()];
			}
		}

		public ParserSymbol this[string name]
		{
			get
			{
				return SymbolsString[name];
			}
		}

		public ParserSymbol this[int index]
		{
			get
			{
				if (index < 0)
				{
					return new ParserSymbol();
				}
				return SymbolsInt[index];
			}
		}
	}

	public class ParserValue
	{
		public string Text;
		public ParserLocation Loc;
		public int Leng = 0;
		public int LineNo = 0;
		
		public ParserValue()
		{
		}
		
		public ParserValue(ParserValue parserValue)
		{
			Text = parserValue.Text;
			Leng = parserValue.Leng;
			Loc = parserValue.Loc;
			LineNo = parserValue.LineNo;
		}
		
		public ParserValue Clone()
		{
			return new ParserValue(this);
		}
	}

	public class JList<T> : List<T> where T : class
	{
		public void Push(T item)
		{
			Add(item);
		}

		public void Pop()
		{
			RemoveAt(Count - 1);
		}

		new public T this[int index]
		{
			get
			{
				if (index >= Count || index < 0 || Count == 0)
				{
					return null;
				}
				return base[index];
			}
		}
	}

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
			_symbolsByName[symbol.Name] = symbol;
			_symbolsByIndex[symbol.Index] = symbol;
		}

		public ParserSymbol Get(string name)
		{
			return _symbolsByName.TryGetValue(name, out var symbol) ? symbol : null;
		}

		public ParserSymbol Get(int index)
		{
			return _symbolsByIndex.TryGetValue(index, out var symbol) ? symbol : null;
		}
	}
}