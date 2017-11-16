using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class BooleanLiteral: IPrimaryExpression
    {
        public bool Value { get; set; }

        public BooleanLiteral(bool value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}