using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Statements
{
    public class ReturnStatement : IStatement
    {
        public Expression Expression { get; set; }
        public ICommonTreeInterface Parent { get; set; }

        public ReturnStatement()
        {
        }

        public ReturnStatement(Expression expression)
        {
            if (expression != null)
            {
                Expression = expression;
                Expression.Parent = this;
            }
        }

        public ReturnStatement(ReturnStatement returnStatement)
        {
            if (returnStatement != null) Expression = new Expression(returnStatement.Expression) {Parent = this};
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
<<<<<<< HEAD
            return $"return {Expression}";
=======
            return Expression.ToString();
>>>>>>> 0d1fc736cafb7c5f92322637b20f2ff810803e03
        }
    }
}