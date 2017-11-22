using System;

namespace Compiler.Exceptions
{
    public class DuplicatedDeclarationException: Exception
    {
        public DuplicatedDeclarationException(string identifier) : base(identifier)
        {
            Message = $"Method/Class/Variable with name or signature \"{identifier}\" is already declared";
        }

        public DuplicatedDeclarationException()
        {
        }

        public override string Message { get; }
    }
}