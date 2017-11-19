using System;
using System.Collections.Generic;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class Expression : ICommonTreeInterface
    {
        // 5.Plus(4) - 5 is a primary part, всё остальное - calls либо fields
        public ICommonTreeInterface Parent { get; set; }

        public string ReturnType { get; set; }
        public IPrimaryExpression PrimaryPart { get; set; }
        public List<MethodOrFieldCall> Calls { get; set; } = new List<MethodOrFieldCall>();

        public Expression(IPrimaryExpression primaryPart)
        {
            PrimaryPart = primaryPart;
            primaryPart.Parent = this;
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
            PrimaryPart.Parent = this;
            foreach (var methodOrFieldCall in Calls)
                methodOrFieldCall.Parent = this;
        }

        public Expression(Expression expression)
        {
            if (expression.ReturnType != null) ReturnType = string.Copy(expression.ReturnType);
            switch (expression.PrimaryPart)
            {
                case ClassName className:
                    PrimaryPart = new ClassName(className) {Parent = this};
                    break;
                case Base @base:
                    PrimaryPart = new Base(@base) {Parent = this};
                    break;
                case BooleanLiteral booleanLiteral:
                    PrimaryPart = new BooleanLiteral(booleanLiteral) {Parent = this};
                    break;
                case IntegerLiteral integerLiteral:
                    PrimaryPart = new IntegerLiteral(integerLiteral) {Parent = this};
                    break;
                case RealLiteral realLiteral:
                    PrimaryPart = new RealLiteral(realLiteral) {Parent = this};
                    break;
                case This @this:
                    PrimaryPart = new This(@this) {Parent = this};
                    break;
            }
            foreach (var methodOrFieldCall in expression.Calls)
                Calls.Add(new MethodOrFieldCall(methodOrFieldCall) {Parent = this});
        }


        public void Accept(IVisitor visitor) => visitor.Visit(this);

        //TODO Add method calls
        public override string ToString() => PrimaryPart.ToString();
    }
}