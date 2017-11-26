using System;

namespace Compiler.Exceptions
{
    public class InvalidReturnParentException : Exception
    {
        public InvalidReturnParentException()
        {
        }

        public InvalidReturnParentException(string message) : base(message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}