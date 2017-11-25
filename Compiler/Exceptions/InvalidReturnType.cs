using System;

namespace Compiler.Exceptions
{
    public class InvalidReturnType : Exception
    {
        public InvalidReturnType()
        {
        }

        public InvalidReturnType(string message) : base(message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}