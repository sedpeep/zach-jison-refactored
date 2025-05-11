using System;

namespace jQuerySheet
{
    public static class ParserExceptionFactory
    {
        public static ParserException CreateException(string message)
        {
            return new ParserException(message, ParserErrorFactory.CreateError(message));
        }

        public static ParserException CreateException(string message, ParserLocation location)
        {
            return new ParserException(message, ParserErrorFactory.CreateError(message, location));
        }

        public static ParserException CreateException(string message, string expected, string found)
        {
            return new ParserException(message, ParserErrorFactory.CreateError(message, expected, found));
        }

        public static ParserException CreateException(string message, ParserLocation location, string expected, string found)
        {
            return new ParserException(message, ParserErrorFactory.CreateError(message, location, expected, found));
        }

        public static ParserException CreateRecoverableException(string message)
        {
            return new ParserException(message, ParserErrorFactory.CreateRecoverableError(message));
        }

        public static ParserException CreateUnrecoverableException(string message)
        {
            return new ParserException(message, ParserErrorFactory.CreateUnrecoverableError(message));
        }

        public static ParserException CreateException(string message, Exception innerException)
        {
            return new ParserException(message, ParserErrorFactory.CreateError(message), innerException);
        }
    }
} 
 