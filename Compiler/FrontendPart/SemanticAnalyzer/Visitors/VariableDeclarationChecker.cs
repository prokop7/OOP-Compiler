using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Compiler.Exceptions;
using Compiler.TreeStructure;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class VariableDeclarationChecker : BaseVisitor
    {
        public override void Visit(VariableDeclaration variable)
        {
            var parent = variable.Parent;
            if (IsDeclared(variable.Parent, variable.Identifier))
                throw new DuplicatedDeclarationException(variable.Identifier);
            switch (parent)
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

        private static bool IsDeclared(ICommonTreeInterface node, string identifier)
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
                    throw new NotImplementedException($"Class not implemented {node.GetType()}");
            }
        }
    }
}