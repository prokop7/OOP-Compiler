using System;

namespace Compiler.Exceptions
{
    public class InvalidReturnTypeException : Exception
    {
        public InvalidReturnTypeException()
        {
        }

        public InvalidReturnTypeException(string message) : base(message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}