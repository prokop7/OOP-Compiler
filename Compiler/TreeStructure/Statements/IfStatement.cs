using System.Collections.Generic;
using System.Reflection;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Statements
{
    public class IfStatement: IStatement
    {
        public Expression Expression { get; set; }
        public List<IBody> Body { get; set; } = new List<IBody>();
        public List<IBody> ElseBody { get; set; } = new List<IBody>();

        public IfStatement(Expression expression, List<IBody> body)
        {
            Expression = expression;
            Body = body;
        }

        public IfStatement(Expression expression, List<IBody> body, List<IBody> elseBody) 
        {
            Expression = expression;
            Body = body;
            ElseBody = elseBody;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}