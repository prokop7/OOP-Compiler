using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Statements
{
    public class ReturnStatement: IStatement
    {
        public Expression Expression { get; set; }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}