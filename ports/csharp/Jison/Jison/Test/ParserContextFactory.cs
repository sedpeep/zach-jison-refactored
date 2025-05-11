using System;

namespace jQuerySheet
{
    public static class ParserContextFactory
    {
        public static ParserContext CreateContext()
        {
            return new ParserContext();
        }

        public static ParserContext CreateContext(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be null or empty", nameof(input));
            }

            var context = new ParserContext();
            context.Ss.Add(new Expression());
            return context;
        }

        public static ParserContext CreateContext(ParserContext template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            var context = new ParserContext();
            context.Ss.AddRange(template.Ss);
            context.Yystate = template.Yystate;
            context.Yy = template.Yy;
            context.ThisS = template.ThisS;
            context.Symbol = template.Symbol;
            context.Action = template.Action;
            context.Location = template.Location;
            return context;
        }
    }
} 