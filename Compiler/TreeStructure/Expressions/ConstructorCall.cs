using System.Collections.Generic;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class ConstructorCall: IPrimaryExpression, ICommonCall
    {
        public ICommonTreeInterface Parent { get; set; }
        public string InputType { get; set; }
        public IMemberDeclaration MemberDeclaration { get; set; }
        public ClassName ClassName { get; set; }
        public List<Expression> Arguments { get; set; } = new List<Expression>();
        public string Type { get; set; }
        
        public ConstructorCall(ClassName className)
        {
            ClassName = className;
            Type = ClassName.Identifier;
        }
        
        public ConstructorCall(LocalCall localCall)
        {
            ClassName = new ClassName(localCall.Identifier) {Parent = this};
            if (localCall.Arguments != null)
                foreach (var argument in localCall.Arguments)
                {
                    Arguments.Add(new Expression(argument) {Parent = this});
                }
            Parent = localCall.Parent;
            Type = ClassName.Identifier;
        }

        public ConstructorCall(ClassName className, List<Expression> arguments) : this(className)
        {
            Arguments = arguments;
            foreach (var expression in arguments)
                expression.Parent = this;
        }

        public ConstructorCall(ConstructorCall call)
        {
            ClassName = new ClassName(call.ClassName);
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

        public override string ToString()
        {
            var args = "";
            Arguments.ForEach(arg => args += $"{arg}, ");
            if(Arguments.Count > 0)
                args = args.Remove(args.Length - 2);
            return ClassName + $"({args})";
        }
    }
}