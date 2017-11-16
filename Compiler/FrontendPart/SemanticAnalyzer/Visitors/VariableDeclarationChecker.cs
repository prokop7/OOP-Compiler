using System;
using System.Diagnostics;
using Compiler.Exceptions;
using Compiler.TreeStructure;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class VariableDeclarationChecker: BaseVisitor
    {
        public override void Visit(VariableDeclaration variable)
        {
            var parent = variable.Parent;
            if (parent is MethodDeclaration)
            {
                var method = parent as MethodDeclaration;
                var methodVarList = method.VariableDeclarations;
                var parentClass = method.Parent as Class;
                Debug.Assert(parentClass != null, nameof(parentClass) + " != null");
                if (methodVarList.ContainsKey(variable.Identifier) || parentClass.Members.ContainsKey(variable.Identifier))
                    throw new DuplicatedDeclarationException(variable.Identifier);
                methodVarList.Add(variable.Identifier, variable);
            }
        }
    }
    
}