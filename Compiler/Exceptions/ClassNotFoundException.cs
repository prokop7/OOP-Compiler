using System;

namespace Compiler.Exceptions
{
    public class ClassNotFoundException: Exception
    {
        public ClassNotFoundException(string message) : base(message)
        {
            Message = message;
        }

        public ClassNotFoundException()
        {
        }

        public override string Message { get; }
    }
}