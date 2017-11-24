using System;
using System.Dynamic;
using Compiler.Exceptions;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class MethodCallsChecker : BaseVisitor
    {
//        public override void Visit(ClassName className)
//        {
//            base.Visit(className);
//            if (!StaticTables.ClassTable.ContainsKey(className.Identifier))
//                throw new ClassNotFoundException(className.Identifier);
//        }

        public override void Visit(FieldCall field)
        {
            base.Visit(field);
            if (!VariableDeclarationChecker.IsDeclared(field, field.Identifier))
                throw new ClassMemberNotFoundException(field.InputType, field.Identifier);
        }
        
        //TODO check method call
        public override void Visit(Call call)
        {
            base.Visit(call);
            GetMethod(call.InputType, call.Identifier);
        }

        public override void Visit(Expression expression)
        {
            if (expression.Calls.Count > 0)
            {
                var inputType = expression.PrimaryPart.Type;
                for (var i = 0; i < expression.Calls.Count; i++)
                {
                    var call = expression.Calls[i];
                    call.InputType = inputType;
                    var callDeclaration = GetMethod(inputType, call.Identifier);
                    switch (callDeclaration)
                    {
                        case ConstructorDeclaration constructorDeclaration:
                            //TODO do something
                            break;
                        case MethodDeclaration methodDeclaration:
                            inputType = methodDeclaration.ResultType.Identifier;
                            if (string.IsNullOrEmpty(inputType) && i < expression.Calls.Count - 1)
                                throw new Exception("Cannot call method from void");
                            break;
                        case VariableDeclaration variableDeclaration:
                            inputType = variableDeclaration.Expression.ReturnType;
                            break;
                    }
                }
            }
        }

        public static IMemberDeclaration GetMethod(string className, string identifier)
        {
            if (!StaticTables.ClassTable.ContainsKey(className)) throw new ClassNotFoundException(className);
            var @class = StaticTables.ClassTable[className][0];
            if (!@class.Members.ContainsKey(identifier)) throw new ClassMemberNotFoundException(className, identifier);
            return @class.Members[identifier];
        }
    }
}