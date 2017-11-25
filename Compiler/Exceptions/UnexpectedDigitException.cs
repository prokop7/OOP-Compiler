using System;

namespace Compiler.Exceptions
{
    public class UnexpectedDigitException: Exception
    {
        public UnexpectedDigitException(string message) : base(message)
        {
            Message = message;
        }

        public UnexpectedDigitException(int line, int position) 
            : base($"Unexpected digit at {line} line, {position} position. ")
        {
            Message = base.Message;
        }

        public UnexpectedDigitException()
        {
        }

        public override string Message { get; }
    }
}