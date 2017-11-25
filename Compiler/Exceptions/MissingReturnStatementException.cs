using System;

namespace Compiler.Exceptions
{
    public class MissingReturnStatementException : Exception
    {
        public MissingReturnStatementException()
        {
        }

        public MissingReturnStatementException(string message) : base(message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}