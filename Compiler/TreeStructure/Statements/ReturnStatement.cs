using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Statements
{
    public class ReturnStatement: IStatement
    {
        public ReturnStatement(){}

        public ReturnStatement(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; set; }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}