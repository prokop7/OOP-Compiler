using System.Collections.Generic;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class Expression: ICommonTreeInterface
    {
        public Expression(IPrimaryExpression primaryPart)
        {
            PrimaryPart = primaryPart;
            if (primaryPart.GetType() == typeof(IntegerLiteral))
                ReturnType = "Integer";
            else if (primaryPart.GetType() == typeof(RealLiteral))
                ReturnType = "Real";
            else if (primaryPart.GetType() == typeof(BooleanLiteral))
                ReturnType = "Boolean";
        }

        public Expression(IPrimaryExpression primaryPart, List<MethodOrFieldCall> calls)
        {
            PrimaryPart = primaryPart;
            Calls = calls;
        }

        public string ReturnType { get; set; }
        public IPrimaryExpression PrimaryPart { get; set; } 
        public List<MethodOrFieldCall> Calls { get; set; } = new List<MethodOrFieldCall>();

        // TODO Ask and then fill in.

        public override string ToString()
        {
            return PrimaryPart.ToString();
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}