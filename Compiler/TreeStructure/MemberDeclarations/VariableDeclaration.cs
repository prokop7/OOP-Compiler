using System.Collections.Generic;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class VariableDeclaration : IMemberDeclaration, IBody
    {
        public VariableDeclaration(string identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }


        public string Identifier { get; set; }
        public Expression Expression { get; set; }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"Var: {Identifier}";
        }
    }
}