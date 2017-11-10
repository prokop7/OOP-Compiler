using System;

namespace Compiler.Exceptions
{
    public class OverflowException: Exception
    {
        public OverflowException(string message) : base(message)
        {
            Message = message;
        }

        public OverflowException()
        {
        }

        public override string Message { get; }
    }
}