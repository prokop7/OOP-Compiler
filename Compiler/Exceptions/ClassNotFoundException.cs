using System;
using System.Reflection;
using Compiler.TreeStructure;

namespace Compiler.Exceptions
{
    public class ClassNotFoundException: Exception
    {
        public ClassNotFoundException(string className) : base(className)
        {
            Message = $"Class {className} not found";
        }

        public ClassNotFoundException()
        {
        }

        public override string Message { get; }
    }
}