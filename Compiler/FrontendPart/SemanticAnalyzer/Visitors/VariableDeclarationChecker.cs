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

        public static bool IsDeclared(ICommonTreeInterface node, string identifier)
        {
            switch (node)
            {
                case null:
                    return false;
                case Class @class:
                    return @class.Members.ContainsKey(identifier);
                case MethodDeclaration method:
                    return method.VariableDeclarations.ContainsKey(identifier)
                           || IsDeclared(node.Parent, identifier);
                case IfStatement ifStatement:
                    return ifStatement.VariableDeclarations.ContainsKey(identifier)
                           || IsDeclared(node.Parent, identifier);
                case WhileLoop whileLoop:
                    return whileLoop.VariableDeclarations.ContainsKey(identifier) ||
                           IsDeclared(node.Parent, identifier);
                case ConstructorDeclaration constructorDeclaration:
                    return constructorDeclaration.VariableDeclarations.ContainsKey(identifier) ||
                           IsDeclared(node.Parent, identifier);
                default:
                    return IsDeclared(node.Parent, identifier);
            }
        }
    }
}