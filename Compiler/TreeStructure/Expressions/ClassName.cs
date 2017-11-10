using System.Collections.Generic;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class ClassName : IPrimaryExpression
    {
        public ClassName(string name)
        {
            Identifier = name;
        }

        public string Identifier { get; set; } = null; // класс от которого наследуется текущий класс
        public List<string> Specification { get; set; } = null; // для дженериков
    }
}