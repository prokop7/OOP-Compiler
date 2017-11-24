using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Statements
{
    public class WhileLoop : IStatement
    {
        public Expression Expression { get; set; }
        public List<IBody> Body { get; set; } = new List<IBody>();
        public ICommonTreeInterface Parent { get; set; }

        public Dictionary<string, VariableDeclaration> VariableDeclarations { get; set; } =
            new Dictionary<string, VariableDeclaration>();

        public WhileLoop(Expression expression, List<IBody> body)
        {
            Expression = expression;
            Body = body;
            Expression.Parent = this;
            foreach (var el in Body)
                el.Parent = this;
        }
        
        public WhileLoop(Expression expression)
        {
            Expression = expression;
            Expression.Parent = this;
        }

        public WhileLoop(WhileLoop whileLoop)
        {
            Expression = new Expression(whileLoop.Expression) {Parent = this};

            foreach (var body in whileLoop.Body)
                SetBody(Body, body);

            void SetBody(ICollection<IBody> bodyList, IBody body)
            {
                switch (body)
                {
                    case VariableDeclaration variableDeclaration:
                        bodyList.Add(new VariableDeclaration(variableDeclaration) {Parent = this});
                        break;
                    case Assignment assignment:
                        bodyList.Add(new Assignment(assignment) {Parent = this});
                        break;
                    case IfStatement @if:
                        bodyList.Add(new IfStatement(@if) {Parent = this});
                        break;
                    case ReturnStatement returnStatement:
                        bodyList.Add(new ReturnStatement(returnStatement) {Parent = this});
                        break;
                    case WhileLoop @while:
                        bodyList.Add(new WhileLoop(@while) {Parent = this});
                        break;
                }
            }
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}