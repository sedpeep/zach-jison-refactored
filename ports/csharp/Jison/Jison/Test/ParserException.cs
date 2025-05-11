using System;

namespace jQuerySheet
{
    public class ParserException : Exception
    {
        public ParserError Error { get; }

        public ParserException(string message, ParserError error) : base(message)
        {
            Error = error;
        }

        public ParserException(string message, ParserError error, Exception innerException) 
            : base(message, innerException)
        {
            Error = error;
        }
    }

    public class ParserError
    {
        public string Message { get; set; }
        public ParserLocation Location { get; set; }
        public string Expected { get; set; }
        public string Found { get; set; }
        public bool Recoverable { get; set; }

        public ParserError()
        {
            Location = new ParserLocation();
            Recoverable = true;
        }

        public override string ToString()
        {
            var message = $"Parse error at {Location}";
            
            if (!string.IsNullOrEmpty(Message))
            {
                message += $": {Message}";
            }

            if (!string.IsNullOrEmpty(Expected))
            {
                message += $"\nExpected: {Expected}";
            }

            if (!string.IsNullOrEmpty(Found))
            {
                message += $"\nFound: {Found}";
            }

            return message;
        }
    }
} 