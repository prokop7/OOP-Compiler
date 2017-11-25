using System;

namespace Compiler.Exceptions
{
    public class OverflowException: Exception
    {
        public OverflowException(string message) : base(message)
        {
            Message = message;
        }
        
        public OverflowException(int line, int position) 
            : base($"Constant at line {line} position {position} is too big. ")
        {
            Message = base.Message;
        }

        public OverflowException()
        {
        }

        public override string Message { get; }
    }
}