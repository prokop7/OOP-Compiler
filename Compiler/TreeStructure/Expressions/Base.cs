using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class Base: IPrimaryExpression
    {
        public void Accept(IVisitor visitor)
        {
            throw new System.NotImplementedException();
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}