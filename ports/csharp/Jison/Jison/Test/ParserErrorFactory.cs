using System;

namespace jQuerySheet
{
    public static class ParserErrorFactory
    {
        public static ParserError CreateError(string message)
        {
            return new ParserError
            {
                Message = message,
                Location = new ParserLocation(),
                Recoverable = true
            };
        }

        public static ParserError CreateError(string message, ParserLocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            return new ParserError
            {
                Message = message,
                Location = location,
                Recoverable = true
            };
        }

        public static ParserError CreateError(string message, string expected, string found)
        {
            return new ParserError
            {
                Message = message,
                Expected = expected,
                Found = found,
                Location = new ParserLocation(),
                Recoverable = true
            };
        }

        public static ParserError CreateError(string message, ParserLocation location, string expected, string found)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            return new ParserError
            {
                Message = message,
                Location = location,
                Expected = expected,
                Found = found,
                Recoverable = true
            };
        }

        public static ParserError CreateRecoverableError(string message)
        {
            return new ParserError
            {
                Message = message,
                Location = new ParserLocation(),
                Recoverable = true
            };
        }

        public static ParserError CreateUnrecoverableError(string message)
        {
            return new ParserError
            {
                Message = message,
                Location = new ParserLocation(),
                Recoverable = false
            };
        }
    }
} 