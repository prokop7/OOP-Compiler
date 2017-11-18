using System;
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

        public Assignment(Assignment assignment)
        {
            Identifier = string.Copy(assignment.Identifier);
            Expression = new Expression(assignment.Expression) {Parent = this};
        }

        public string Identifier { get; set; }
        public Expression Expression { get; set; }
        // a = true
        //  a - identifier
        //  true - expression

        public override string ToString()
        {
            return $"{Identifier}:= {Expression}";
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}
