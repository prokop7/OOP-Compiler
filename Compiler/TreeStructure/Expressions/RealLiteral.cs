using System.Globalization;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class RealLiteral : IPrimaryExpression
    {
        public double Value { get; set; }

        public RealLiteral(double value)
        {
            Value = value;
        }

        public RealLiteral(RealLiteral realLiteral)
        {
            Value = realLiteral.Value;
        }

        public override string ToString()
        {
            return Value.ToString(new CultureInfo(0));
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}