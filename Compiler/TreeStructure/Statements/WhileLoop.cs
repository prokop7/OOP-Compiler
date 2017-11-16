using System.Collections.Generic;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Statements
{
    public class WhileLoop: IStatement
    {
        public WhileLoop(Expression expression, List<IBody> body)
        {
            Expression = expression;
            Body = body;
        }
        
        public Expression Expression { get; set; }
        public List<IBody> Body { get; set; }
        
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}