using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class BooleanLiteral : IPrimaryExpression
    {
        public ICommonTreeInterface Parent { get; set; }
        public bool Value { get; set; }

        public BooleanLiteral(bool value)
        {
            Value = value;
        }

        public BooleanLiteral(BooleanLiteral booleanLiteral)
        {
            Value = booleanLiteral.Value;
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString() => Value.ToString();
    }
}