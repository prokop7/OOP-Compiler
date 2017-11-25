using System;

namespace Compiler.Exceptions
{
    public class ClassMemberNotFoundException : Exception
    {
        public ClassMemberNotFoundException(string className, string callIdentifier)
        {
            Message = $"Class member \"{callIdentifier}\" inside \"{className}\" was not found";
        }

        public ClassMemberNotFoundException()
        {
        }

        public override string Message { get; }
    }
}