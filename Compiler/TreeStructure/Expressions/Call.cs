using System.Collections.Generic;
using System.Linq;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class Call : ICall
    {
        public ICommonTreeInterface Parent { get; set; }
        public string InputType { get; set; }
        public IMemberDeclaration MemberDeclaration { get; set; }
        public string Identifier { get; set; }
        public List<Expression> Arguments { get; set; } = new List<Expression>();

        public Call(string identifier)
        {
            Identifier = identifier;
        }

        public Call(string identifier, List<Expression> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
            foreach (var expression in arguments)
                expression.Parent = this;
        }

        public Call(Call call)
        {
            Identifier = string.Copy(call.Identifier);
            if (call.InputType != null) InputType = string.Copy(call.InputType);
            switch (call.MemberDeclaration)
            {
                case ConstructorDeclaration constructorDeclaration:
                    MemberDeclaration = new ConstructorDeclaration(constructorDeclaration);
                    break;
                case MethodDeclaration methodDeclaration:
                    MemberDeclaration = new MethodDeclaration(methodDeclaration);
                    break;
                case VariableDeclaration variableDeclaration:
                    MemberDeclaration = new VariableDeclaration(variableDeclaration);
                    break;
            }
            foreach (var expression in call.Arguments)
                Arguments.Add(new Expression(expression) {Parent = this});
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString() =>
            Identifier + $"({Arguments.Aggregate("", (current, p) => current + (p + ", "))})";
    }
}