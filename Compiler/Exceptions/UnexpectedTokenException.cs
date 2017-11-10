using System;

namespace Compiler.Exceptions
{
    public class UnexpectedTokenException: Exception
    {
        public UnexpectedTokenException(string message) : base(message)
        {
            Message = message;
        }

        public UnexpectedTokenException(int line, int position, string expectation) : base($"Unexpected token at {line} line, {position} position. Expected {expectation}. ")
        {
            Message = $"Unexpected token at {line} line, {position} position. Expected {expectation}. ";
        }

        public UnexpectedTokenException(int line, int position) : base($"Unexpected token at {line} line, {position} position. ")
        {
            Message = $"Unexpected token at {line} line, {position} position. ";
        }
        
        public UnexpectedTokenException()
        {
        }

        public override string Message { get; }
    }
}