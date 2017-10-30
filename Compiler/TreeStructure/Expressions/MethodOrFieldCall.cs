using System.Collections.Generic;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class MethodOrFieldCall: ICommonTreeInterface
    {
        public string Identifier { get; set; }
        public List<Expression> Arguments { get; set; } = new List<Expression>();
        
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}