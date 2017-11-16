using System;
using System.Collections.Generic;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class ClassName : IPrimaryExpression
    {
        public ClassName(string name)
        {
            Identifier = name;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string Identifier { get; set; } = null; // класс от которого наследуется текущий класс
        public List<string> Specification { get; set; } = null; // для дженериков

        public static implicit operator ClassName(string s)
        {
            return new ClassName(s);
        }
        public static implicit operator string(ClassName className)
        {
            return className.Identifier;
        }
    }
}