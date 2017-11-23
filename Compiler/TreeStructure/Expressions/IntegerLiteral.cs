using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class IntegerLiteral : IPrimaryExpression
    {
        public ICommonTreeInterface Parent { get; set; }
        public int Value { get; set; }
        public string Type { get; set; } = "Integer";


        public IntegerLiteral(int value)
        {
            Value = value;
        }

        public IntegerLiteral(IntegerLiteral integerLiteral)
        {
            Value = integerLiteral.Value;
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString() => Value.ToString();
    }
}