using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class This: IPrimaryExpression
    {
        public void Accept(IVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}