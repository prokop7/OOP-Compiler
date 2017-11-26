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
        public override void Visit(LocalCall localCall)
        {
            if (localCall.Parameters == null)
            {
                var variable = (IVariableDeclaration) VariableDeclarationChecker.GetTypeVariable(localCall, localCall.Identifier);
                switch (variable)
                {
                    case VariableDeclaration variableDeclaration:
                        localCall.Type = variableDeclaration.Expression.ReturnType;
                        break;
                    case ParameterDeclaration parameterDeclaration:
                        localCall.Type = parameterDeclaration.Type.Identifier;
                        break;
                    default:
                        throw new VariableNotFoundException(localCall.ToString());
                }
                return;
            }
            var parent = localCall.Parent;
            while (parent != null && !(parent is Class))
                parent = parent.Parent;
            if (parent == null)
                throw new Exception("WTF!?");
            var method =
                GetMethod(((Class) parent).SelfClassName.Identifier, localCall.Identifier) as MethodDeclaration;
            localCall.Type = method?.ResultType.Identifier;
        }

        public override void Visit(FieldCall field)
        {
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
            expression.PrimaryPart.Accept(this);
            var inputType = expression.PrimaryPart.Type;
            expression.ReturnType = inputType;
            if (expression.Calls.Count > 0)
            {
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
                            inputType = methodDeclaration.ResultType?.Identifier;
                            expression.ReturnType = inputType;
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