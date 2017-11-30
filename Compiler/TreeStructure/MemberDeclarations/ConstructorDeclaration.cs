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
        private List<ParameterDeclaration> _parameters  = new List<ParameterDeclaration>();

        public List<ParameterDeclaration> Parameters
        {
            get => _parameters;
            set
            {
                _parameters = value;
                _parameters?.ForEach(param => param.Parent = this);
            }
        }

        public List<IBody> Body { get; set; } = new List<IBody>();

        public Dictionary<string, IVariableDeclaration> VariableDeclarations { get; set; } =
            new Dictionary<string, IVariableDeclaration>();
        
        public Dictionary<string, string> NameMap { get; set; } = new Dictionary<string, string>();

        public ConstructorDeclaration(List<ParameterDeclaration> parameters, List<IBody> bodies)
        {
            if (bodies != null)
                Body = bodies;
            if (parameters != null)
                Parameters = parameters;
        }

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
                    case Expression expression:
                        expression.Parent = this;
                        bodyList.Add(new Expression(expression){Parent = this});
                        break;
                }
            }
            
            foreach (var keyValuePair in constructorDeclaration.NameMap)
                NameMap.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
        
        public override string ToString()
        {
            var body = $"this ({Parameters.Aggregate("", (current, p) => current + (p + ", "))}) is" 
                       + $"{Body.Aggregate("\n", (current, p) => current + (p + "\n"))}end";
            return body;
        }
    }
}