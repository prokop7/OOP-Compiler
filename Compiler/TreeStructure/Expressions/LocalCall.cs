using System.Collections.Generic;
using System.Linq;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class LocalCall: IPrimaryExpression, ICommonCall
    {
        //BUG might happen when you copy. 
        public ICommonTreeInterface Parent { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }
        public List<Expression> Parameters { get; set; } = null;

        public LocalCall(string identifier)
        {
            Identifier = identifier;
        }
        
        public LocalCall(string identifier, List<Expression> patameters) : this(identifier)
        {
            Parameters = patameters;
            foreach (var expression in patameters)
                expression.Parent = this;
        }
        
        


        public void Accept(IVisitor visitor) => visitor.Visit(this);
        
        public override string ToString()
        {
            return Identifier + (Parameters == null ? "" : $"({Parameters.Aggregate("", (current, p) => current + (p + ", "))})");
        }
    }
}