using System;
using System.Collections.Generic;

namespace jQuerySheet
{
    public static class ExpressionFactory
    {
        public static Expression CreateExpression()
        {
            return new Expression();
        }

        public static Expression CreateNumberExpression(double value)
        {
            return new Expression("number", value);
        }

        public static Expression CreateStringExpression(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(value));
            }
            return new Expression("string", value);
        }

        public static Expression CreateIdentifierExpression(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            }
            return new Expression("identifier", name, new List<Expression>());
        }

        public static Expression CreateCallExpression(string name, List<Expression> arguments)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            }
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }
            return new Expression("call", name, arguments);
        }

        public static Expression CreateBinaryExpression(Expression left, string op, Expression right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }
            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }
            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentException("Operator cannot be null or empty", nameof(op));
            }
            return new Expression("binary", left, op, right);
        }

        public static Expression CreateAdditionExpression(Expression left, Expression right)
        {
            return CreateBinaryExpression(left, "+", right);
        }

        public static Expression CreateSubtractionExpression(Expression left, Expression right)
        {
            return CreateBinaryExpression(left, "-", right);
        }

        public static Expression CreateMultiplicationExpression(Expression left, Expression right)
        {
            return CreateBinaryExpression(left, "*", right);
        }

        public static Expression CreateDivisionExpression(Expression left, Expression right)
        {
            return CreateBinaryExpression(left, "/", right);
        }
    }
} 