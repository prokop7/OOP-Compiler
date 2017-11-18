using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class IntegerLiteral: IPrimaryExpression
    {
        public IntegerLiteral(int value)
        {
            Value = value;
        }

        public IntegerLiteral(IntegerLiteral integerLiteral)
        {
            Value = integerLiteral.Value;
        }

        public int Value { get; set; }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}