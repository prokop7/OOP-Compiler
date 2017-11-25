using System.Globalization;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class RealLiteral : IPrimaryExpression
    {
        public ICommonTreeInterface Parent { get; set; }
        public double Value { get; set; }
        public string Type { get; set; } = "Real";


        public RealLiteral(double value)
        {
            Value = value;
        }

        public RealLiteral(RealLiteral realLiteral)
        {
            Value = realLiteral.Value;
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString() => Value.ToString();
    }
}