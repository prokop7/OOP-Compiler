using Compiler.Exceptions;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class VariableDeclarationChecker : BaseVisitor
    {
        public override void Visit(Assignment assignment)
        {
            base.Visit(assignment);
            if (!IsDeclared(assignment, assignment.Identifier))
                throw new VariableNotFoundException(assignment.Identifier);
        }

        public override void Visit(This @this)
        {
            // TODO checking identifier
        }
        
        #region Variable Declaring logic

        public override void Visit(ParameterDeclaration parameter)
        {
            if (IsDeclared(parameter, parameter.Identifier))
                throw new DuplicatedDeclarationException(parameter.Identifier);
            switch (parameter.Parent)
            {
                case ConstructorDeclaration constructorDeclaration:
                    constructorDeclaration.VariableDeclarations.Add(parameter.Identifier, parameter);
                    break;
                case MethodDeclaration methodDeclaration:
                    methodDeclaration.VariableDeclarations.Add(parameter.Identifier, parameter);
                    break;
            }
        }


        public override void Visit(VariableDeclaration variable)
        {
            if (IsDeclared(variable, variable.Identifier))
                throw new DuplicatedDeclarationException(variable.Identifier);
            switch (variable.Parent)
            {
                case MethodDeclaration method:
                    method.VariableDeclarations.Add(variable.Identifier, variable);
                    break;
                case IfStatement ifStatement:
                    ifStatement.VariableDeclarations.Add(variable.Identifier, variable);
                    break;
                case WhileLoop whileLoop:
                    whileLoop.VariableDeclarations.Add(variable.Identifier, variable);
                    break;
                case ConstructorDeclaration constructorDeclaration:
                    constructorDeclaration.VariableDeclarations.Add(variable.Identifier, variable);
                    break;
            }
        }

        #endregion


        public static ICommonTreeInterface GetTypeVariable(ICommonTreeInterface node, string identifier)
        {
            while (true)
            {
                switch (node)
                {
                    case null:
                        return null;
                    case Class @class:
                        return @class.Members.ContainsKey(identifier) ? @class.Members[identifier] : null;
                    case MethodDeclaration method:
                        if (method.VariableDeclarations.ContainsKey(identifier))
                            return method.VariableDeclarations[identifier];
                        break;
                    case IfStatement ifStatement:
                        if (ifStatement.VariableDeclarations.ContainsKey(identifier))
                            return ifStatement.VariableDeclarations[identifier];
                        break;
                    case WhileLoop whileLoop:
                        if (whileLoop.VariableDeclarations.ContainsKey(identifier))
                            return whileLoop.VariableDeclarations[identifier];
                        break;
                    case ConstructorDeclaration constructorDeclaration:
                        if (constructorDeclaration.VariableDeclarations.ContainsKey(identifier))
                            return constructorDeclaration.VariableDeclarations[identifier];
                        break;
                }
                node = node.Parent;
            }
        }

        public static bool IsDeclared(ICommonTreeInterface node, string identifier)
        {
            var res = GetTypeVariable(node, identifier);
            return res != null;
        }
    }
}