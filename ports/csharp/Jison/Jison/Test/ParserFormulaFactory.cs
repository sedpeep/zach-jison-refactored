using System;

namespace jQuerySheet
{
    public static class ParserFormulaFactory
    {
        public static Formula CreateFormula()
        {
            return new Formula();
        }

        public static Formula CreateFormula(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be null or empty", nameof(input));
            }

            var formula = new Formula();
            formula.Parse(input);
            return formula;
        }

        public static Formula CreateFormula(Formula template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            var formula = new Formula();
            // Copy state from template if needed
            return formula;
        }

        public static Formula CreateDefaultFormula()
        {
            var formula = new Formula();
            // Initialize with default state
            return formula;
        }
    }
} 