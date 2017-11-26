using System.Collections.Generic;
using System.Linq;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Statements;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class ConstructorDeclaration : IMemberDeclaration
    {
        public ICommonTreeInterface Parent { get; set; }
        public List<ParameterDeclaration> Parameters { get; set; } = new List<ParameterDeclaration>();
        public List<IBody> Body { get; set; } = new List<IBody>();

        public Dictionary<string, IVariableDeclaration> VariableDeclarations { get; set; } =
            new Dictionary<string, IVariableDeclaration>();

        public Dictionary<string, string> NameMap { get; set; } = new Dictionary<string, string>();

        public ConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
        {
            foreach (var body in constructorDeclaration.Body)
                SetBody(Body, body);
            foreach (var parameter in constructorDeclaration.Parameters)
            {
                Parameters.Add(new ParameterDeclaration(parameter) {Parent = this});
            }


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

            foreach (var keyValuePair in constructorDeclaration.NameMap)
                NameMap.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public ConstructorDeclaration(List<ParameterDeclaration> parameters, List<IBody> bodies)
        {
            if (bodies != null)
            {
                foreach (var body in bodies)
                    SetBody(Body, body);
            }
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    parameter.Parent = this;
                    Parameters.Add(parameter);
                }
            }

            void SetBody(ICollection<IBody> bodyList, IBody body)
            {
                switch (body)
                {
                    case Expression expression:
                        expression.Parent = this;
                        bodyList.Add(expression);
                        break;
                    case VariableDeclaration variableDeclaration:
                        variableDeclaration.Parent = this;
                        bodyList.Add(variableDeclaration);
                        break;
                    case Assignment assignment:
                        assignment.Parent = this;
                        bodyList.Add(assignment);
                        break;
                    case IfStatement @if:
                        @if.Parent = this;
                        bodyList.Add(@if);
                        break;
                    case ReturnStatement returnStatement:
                        returnStatement.Parent = this;
                        bodyList.Add(returnStatement);
                        break;
                    case WhileLoop whileLoop:
                        whileLoop.Parent = this;
                        bodyList.Add(whileLoop);
                        break;
                        
                }
            }
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            var body =
                $"{((Class) Parent).SelfClassName.Identifier}({Parameters.Aggregate("", (current, p) => current + (p + ", "))})";
            return body;
        }
    }
}