using System;

namespace jQuerySheet
{
    public static class ParserValueFactory
    {
        public static ParserValue CreateValue()
        {
            return new ParserValue();
        }

        public static ParserValue CreateValue(string text)
        {
            var value = new ParserValue();
            value.Set(text);
            return value;
        }

        public static ParserValue CreateValue(double number)
        {
            var value = new ParserValue();
            value.Set(number);
            return value;
        }

        public static ParserValue CreateValue(bool boolean)
        {
            var value = new ParserValue();
            value.Set(boolean);
            return value;
        }

        public static ParserValue CreateValue(ParserValue template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            return template.Clone();
        }

        public static ParserValue CreateNumericValue(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text cannot be null or empty", nameof(text));
            }

            var value = new ParserValue();
            if (double.TryParse(text, out double number))
            {
                value.Set(number);
            }
            else
            {
                throw new ArgumentException($"'{text}' is not a valid number");
            }
            return value;
        }

        public static ParserValue CreateBooleanValue(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Text cannot be null or empty", nameof(text));
            }

            var value = new ParserValue();
            if (bool.TryParse(text, out bool boolean))
            {
                value.Set(boolean);
            }
            else
            {
                throw new ArgumentException($"'{text}' is not a valid boolean");
            }
            return value;
        }
    }
} 