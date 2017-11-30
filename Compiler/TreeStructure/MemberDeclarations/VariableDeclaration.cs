using System.Collections.Generic;
using System.Runtime.Serialization;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class VariableDeclaration : IMemberDeclaration, IBody, IVariableDeclaration
    {
        public string Identifier { get; set; } // название
        public Expression Expression { get; set; } // var i = expresion. Это и есть expression
        public ClassName Classname { get; set; } // инициализируется парсером при явном указании типа переменной
        public ICommonTreeInterface Parent { get; set; }
        public bool IsDeclared { get; set; } = true;
        
        public VariableDeclaration(string identifier)
        {
            Identifier = identifier;
        }
        
        public VariableDeclaration(string identifier, Expression expression) : this(identifier)
        {
            Expression = expression;
            Expression.Parent = this;
        }
        
        public VariableDeclaration(string identifier, ClassName className, Expression expression) : this(identifier, expression)
        {
            Classname = className;
            className.Parent = this;
        }

        public VariableDeclaration(VariableDeclaration variableDeclaration)
        {
            Identifier = string.Copy(variableDeclaration.Identifier);
            Expression = new Expression(variableDeclaration.Expression) {Parent = this};

            if (variableDeclaration.Classname != null) Classname = new ClassName(variableDeclaration.Classname);
        }


        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            return $"var {Identifier} : {Classname} is {Expression}";
        }

    }
}