using System.Collections.Generic;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class ConstructorCall: IPrimaryExpression
    {
        public ICommonTreeInterface Parent { get; set; }
        public string InputType { get; set; }
        public ClassName ClassName { get; set; }
        public List<Expression> Arguments { get; set; } = new List<Expression>();
        public string Type { get; set; }
        
        public ConstructorCall(ClassName className)
        {
            ClassName = className;
        }

        public ConstructorCall(ClassName className, List<Expression> arguments)
        {
            ClassName = className;
            Arguments = arguments;
            foreach (var expression in arguments)
                expression.Parent = this;
        }

        public ConstructorCall(ConstructorCall call)
        {
            ClassName = new ClassName(call.ClassName);
            if (call.InputType != null) InputType = string.Copy(call.InputType);
            foreach (var expression in call.Arguments)
                Arguments.Add(new Expression(expression) {Parent = this});
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}