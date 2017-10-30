using System.Collections.Generic;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Statements
{
    public class Assignment: IStatement
    {
        public Assignment(string identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public string Identifier { get; set; }
        public Expression Expression { get; set; }

        public override string ToString()
        {
            return $"{Identifier}:= {Expression}";
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}