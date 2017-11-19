using System.Collections.Generic;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    // 5.Plus(4) - Plus is Identifier, 4 is Argument
    public class MethodOrFieldCall : ICommonTreeInterface
    {
        public ICommonTreeInterface Parent { get; set; }
        public string Identifier { get; set; }
        public List<Expression> Arguments { get; set; } = new List<Expression>();

        public MethodOrFieldCall(string identifier)
        {
            Identifier = identifier;
        }

        public MethodOrFieldCall(string identifier, List<Expression> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
            foreach (var expression in arguments)
                expression.Parent = this;
        }

        public MethodOrFieldCall(MethodOrFieldCall methodOrFieldCall)
        {
            Identifier = string.Copy(methodOrFieldCall.Identifier);
            foreach (var expression in methodOrFieldCall.Arguments)
                Arguments.Add(new Expression(expression) {Parent = this});
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}