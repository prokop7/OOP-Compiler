using System.Collections.Generic;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class LocalCall: IPrimaryExpression, ICommonCall
    {
        public ICommonTreeInterface Parent { get; set; }
        public string Identifier { get; set; }
        public List<Expression> Arguments { get; set; }
        public string Type { get; set; }
        public string InputType { get; set; }
        public IMemberDeclaration MemberDeclaration { get; set; }

        public LocalCall(string identifier)
        {
            Identifier = identifier;
        }
        
        public LocalCall(string identifier, List<Expression> arguments) : this(identifier)
        {
            Arguments = arguments;
            foreach (var expression in arguments)
                expression.Parent = this;
        }
        
        

        public override string ToString()
        {
            var args = "";
            if (Arguments != null)
            {
                args += "(";
                Arguments.ForEach(arg => args += $"{arg}, ");
                    args += ")";
                if(Arguments.Count > 0)
                    args = args.Remove(args.Length - 2);
            }
            return Identifier + $"{args}";
        }


        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}