using System;
using Compiler.FrontendPart;

namespace Compiler.Exceptions
{
    public class UnexpectedTokenException: Exception
    {
        public UnexpectedTokenException(string message) : base(message)
        {
            Message = message;
        }

        public UnexpectedTokenException(int line, int position, string expectation = null) 
            : base($"Unexpected token at {line} line, {position} position. Expected {expectation}. "
                   + (expectation == null ? "" : $"Expected {expectation}"))
        {
            Message = base.Message;
        }

/*        public UnexpectedTokenException(int line, int position) 
            : base($"Unexpected token at {line} line, {position} position. ")
        {
            Message = base.Message;
        }*/
        
        public UnexpectedTokenException(Token token, string expectation = null)
            : base($"Unexpected token {token.ToString()} at {token.lineNumber} line, {token.positionNumber} position. " 
                   + (expectation == null ? "" : $"Expected {expectation}"))
        {
            Message = base.Message;
        }
        
        public UnexpectedTokenException()
        {
        }

        public override string Message { get; }
    }
}