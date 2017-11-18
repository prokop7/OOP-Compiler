using System.Collections.Generic;
using System.Runtime.Serialization;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class VariableDeclaration : IMemberDeclaration, IBody, IVariableDeclaration
    {
        public VariableDeclaration(string identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
            expression.Parent = this;
        }

        public VariableDeclaration(VariableDeclaration variableDeclaration)
        {
            Identifier = string.Copy(variableDeclaration.Identifier);
            Expression = new Expression(variableDeclaration.Expression) {Parent = this};

            if (variableDeclaration.ClassName != null) ClassName = string.Copy(variableDeclaration.ClassName);
        }

        public string Identifier { get; set; } // название
        public Expression Expression { get; set; } // var i = expresion. Это и есть expression
        public string ClassName { get; set; } // инициализируется парсером при явном указании типа переменной

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"var {Identifier}: {Expression}";
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}