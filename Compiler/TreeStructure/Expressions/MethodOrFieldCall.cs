using System.Collections.Generic;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class MethodOrFieldCall: ICommonTreeInterface
    {
        public MethodOrFieldCall(string identifier)
        {
            Identifier = identifier;
        }

        public MethodOrFieldCall(string identifier, List<Expression> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
        }

        public MethodOrFieldCall(MethodOrFieldCall methodOrFieldCall)
        {
            Identifier = string.Copy(methodOrFieldCall.Identifier);
            foreach (var expression in methodOrFieldCall.Arguments)
            {
                Arguments.Add(new Expression(expression) {Parent = this});
            }
        }

        public string Identifier { get; set; }
        public List<Expression> Arguments { get; set; } = new List<Expression>();
        // 5.Plus(4) - Plus is Identifier, 4 is Argument
        
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}
