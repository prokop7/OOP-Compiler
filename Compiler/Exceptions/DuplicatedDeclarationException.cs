using System;

namespace Compiler.Exceptions
{
    public class DuplicatedDeclarationException: Exception
    {
        public DuplicatedDeclarationException(string identifier) : base(identifier)
        {
            Message = $"Name: \"{identifier}\" is already declared";
        }

        public DuplicatedDeclarationException()
        {
        }

        public override string Message { get; }
    }
}