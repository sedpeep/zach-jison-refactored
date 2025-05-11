using System;

namespace jQuerySheet
{
    public static class ParserLocationFactory
    {
        public static ParserLocation CreateLocation()
        {
            return new ParserLocation();
        }

        public static ParserLocation CreateLocation(int line, int column)
        {
            return new ParserLocation
            {
                Line = line,
                Column = column
            };
        }

        public static ParserLocation CreateLocation(ParserLocation template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            return new ParserLocation
            {
                Line = template.Line,
                Column = template.Column
            };
        }

        public static ParserLocation CreateLocation(string text, int position)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text cannot be null or empty", nameof(text));
            }

            if (position < 0 || position > text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            var line = 1;
            var column = 1;

            for (int i = 0; i < position; i++)
            {
                if (text[i] == '\n')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
            }

            return new ParserLocation
            {
                Line = line,
                Column = column
            };
        }
    }
} 