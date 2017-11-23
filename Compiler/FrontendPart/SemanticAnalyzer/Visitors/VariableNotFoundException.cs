using System;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class VariableNotFoundException : Exception
    {
        public VariableNotFoundException(string identifier)
        {
            Message = $"Variable \"{identifier}\" not found.";
        }

        public override string Message { get; }
    }
}