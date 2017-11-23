using System;
using System.Collections.Generic;
using Compiler.Exceptions;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class GenericTypesCheck : BaseVisitor
    {
        public Class Class { get; set; }
        public List<string> GenericTypes { get; set; }

        public GenericTypesCheck(GenericClass @class)
        {
            GenericTypes = @class.GenericParams;
            Class = @class;
        }

        public override void Visit(Expression expression)
        {
            base.Visit(expression);
            if (expression.Calls.Count == 0)
                return;
            var type = "";
            switch (expression.PrimaryPart)
            {
                case ClassName className:
                    type = className.Identifier;
                    break;
                case Base @base:
                    type = Class.BaseClassName.Identifier;
                    break;
                case BooleanLiteral booleanLiteral:
                    type = "Boolean";
                    break;
                case IntegerLiteral integerLiteral:
                    type = "Integer";
                    break;
                case RealLiteral realLiteral:
                    type = "Real";
                    break;
                case This @this:
                    type = Class.SelfClassName.Identifier;
                    break;
            }
            for (var i = 0; i < expression.Calls.Count; i++)
            {
                if (GenericTypes.Contains(type))
                    throw new ClassMemberNotFoundException();
                type = GetType(type, expression.Calls[i].Identifier);
            }
            
            string GetType(string classIdentifier, string identifier)
            {
                Class @class = null;
                if (StaticTables.ClassTable.ContainsKey(classIdentifier))
                {
                    // TODO remove [0] 
                    @class = StaticTables.ClassTable[classIdentifier][0];
                }
                if (StaticTables.GenericClassTable.ContainsKey(classIdentifier))
                {
                    // TODO remove [0]
                    @class = StaticTables.GenericClassTable[classIdentifier][0];
                }
                if (@class == null)
                    throw new ClassNotFoundException();
                
                if (Class.Members.ContainsKey(identifier))
                {
                    var memberDeclaration = Class.Members[identifier];
                    switch (memberDeclaration)
                    {
                        case ConstructorDeclaration _:
                            return classIdentifier;
                        case MethodDeclaration methodDeclaration:
                            return methodDeclaration.ResultType;
                        case VariableDeclaration variableDeclaration:
                            return variableDeclaration.Expression.ReturnType;
                    }
                    return classIdentifier;
                }
                throw new ClassMemberNotFoundException();
            }
            
            
            
            
//            if (!(expression.PrimaryPart is ClassName className)) return;
//            if (expression.Calls.Count > 0)
//                if (GenericTypes.Contains(className.Identifier))
//                    throw new ClassMemberNotFoundException();
//                else
//                {
//                    var primaryPart = expression.PrimaryPart;
//                    for (var i = 0; i < expression.Calls.Count - 1; i++)
//                    {
//                    }
//                }
        }

        public static ICommonTreeInterface GetParentClass(ICommonTreeInterface node)
        {
            while (true)
            {
                if (node is Class) return node;
                node = node.Parent;
            }
        }
    }
    
}