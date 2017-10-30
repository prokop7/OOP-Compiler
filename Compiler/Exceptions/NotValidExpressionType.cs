using System;

namespace Compiler.Exceptions
{
    public class NotValidExpressionTypeException: Exception
    {
        public NotValidExpressionTypeException()
        {
        }

        public NotValidExpressionTypeException(string message) : base(message)
        {
            Message = message;
        }

        public override string Message { get; }
    }
}