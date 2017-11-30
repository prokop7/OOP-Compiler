using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Statements
{
    public class IfStatement : IStatement
    {
        public ICommonTreeInterface Parent { get; set; }
        public Expression Expression { get; set; }
        private List<IBody> _body = new List<IBody>();
        private List<IBody> _elseBody = new List<IBody>();
        
        public List<IBody> Body
        {
            get => _body;
            set
            {
                _body = value;
                _body?.ForEach(body => body.Parent = this);
            }
        }

        public List<IBody> ElseBody
        {
            get => _elseBody;
            set
            {
                _elseBody = value;
                _elseBody?.ForEach(body => body.Parent = this);
            }
        }

        public Dictionary<string, VariableDeclaration> VariableDeclarations { get; set; } =
            new Dictionary<string, VariableDeclaration>();
        
        public Dictionary<string, string> NameMap { get; set; } = new Dictionary<string, string>();

        // if expression is true, выполняется Body, else - elsebody
        public IfStatement(Expression expression, List<IBody> body)
        {
            Expression = expression;
            Expression.Parent = this;

            if (body == null) return;
            Body = body;
        }

        public IfStatement(Expression expression, List<IBody> body, List<IBody> elseBody) : this(expression, body)
        {
            if (elseBody == null) return;
            ElseBody = elseBody;
        }

        public IfStatement(IfStatement ifStatement)
        {
            Expression = new Expression(ifStatement.Expression) {Parent = this};
            foreach (var body in ifStatement.Body)
                SetBody(Body, body);
            foreach (var body in ifStatement.ElseBody)
                SetBody(ElseBody, body);

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
                    case WhileLoop whileLoop:
                        bodyList.Add(new WhileLoop(whileLoop) {Parent = this});
                        break;
                }
            }
            
            foreach (var keyValuePair in ifStatement.NameMap)
                NameMap.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            return $"if ({Expression}) \nthen \n{Body.Aggregate("", (current, p) => current + (p + "\n"))}" 
                + (ElseBody.Count == 0 ? "" : $"else\n{ElseBody.Aggregate("", (current, p) => current + (p + "\n"))}") + "end";
        }
    }
}