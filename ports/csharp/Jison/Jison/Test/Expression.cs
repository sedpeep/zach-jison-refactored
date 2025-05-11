using System;
using System.Collections.Generic;

namespace jQuerySheet
{
	public class Expression
	{
		public string Type { get; set; }
		public object Value { get; set; }
		public List<Expression> Arguments { get; set; }
		public string Name { get; set; }
		public Expression Left { get; set; }
		public Expression Right { get; set; }
		public string Operator { get; set; }

		public Expression()
		{
			Arguments = new List<Expression>();
		}

		public Expression(string type, object value)
		{
			Type = type;
			Value = value;
			Arguments = new List<Expression>();
		}

		public Expression(string type, string name, List<Expression> args)
		{
			Type = type;
			Name = name;
			Arguments = args ?? new List<Expression>();
		}

		public Expression(string type, Expression left, string op, Expression right)
		{
			Type = type;
			Left = left;
			Operator = op;
			Right = right;
			Arguments = new List<Expression>();
		}

		public object Evaluate()
		{
			switch (Type)
			{
				case "number":
					return Convert.ToDouble(Value);
				case "string":
					return Value.ToString();
				case "identifier":
					return Name;
				case "call":
					return EvaluateCall();
				case "binary":
					return EvaluateBinary();
				default:
					throw new InvalidOperationException($"Unknown expression type: {Type}");
			}
		}

		private object EvaluateCall()
		{
			// Implementation of function call evaluation
			return null;
		}

		private object EvaluateBinary()
		{
			var leftValue = Left.Evaluate();
			var rightValue = Right.Evaluate();

			switch (Operator)
			{
				case "+":
					return Convert.ToDouble(leftValue) + Convert.ToDouble(rightValue);
				case "-":
					return Convert.ToDouble(leftValue) - Convert.ToDouble(rightValue);
				case "*":
					return Convert.ToDouble(leftValue) * Convert.ToDouble(rightValue);
				case "/":
					return Convert.ToDouble(leftValue) / Convert.ToDouble(rightValue);
				default:
					throw new InvalidOperationException($"Unknown operator: {Operator}");
			}
		}
	}
}

