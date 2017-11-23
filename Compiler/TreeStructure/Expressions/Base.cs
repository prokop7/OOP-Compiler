using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class Base: IPrimaryExpression
    {
        public ICommonTreeInterface Parent { get; set; }

        public Base(Base @base)
        {
        }

        public Base()
        {
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}