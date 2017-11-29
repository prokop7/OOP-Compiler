using System;
using System.Dynamic;
using System.Linq;
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
            base.Visit(localCall);
            if (localCall.Arguments == null)
            {
                var nName = VariableDeclarationChecker.GetValueFromMap(localCall, localCall.Identifier);
                if (nName != null)
                    localCall.Identifier = nName;
                var variable =
                    (IVariableDeclaration) VariableDeclarationChecker.GetTypeVariable(localCall, localCall.Identifier);
                if (variable == null)
                {
                    var p = localCall.Parent;
                    while (p != null && !(p is Class))
                        p = p.Parent;
                    if (p == null)
                        throw new Exception("WTF!?");
                    var cls = (Class) p;
                    check:
                    if (cls.Base != null && !cls.NameMap.ContainsKey(localCall.Identifier) &&
                        !cls.Members.ContainsKey(localCall.Identifier))
                    {
                        cls = cls.Base;
                        goto check;
                    }
                    if (cls.NameMap.ContainsKey(localCall.Identifier))
                        localCall.Identifier = cls.NameMap[localCall.Identifier];
                    if (!cls.Members.ContainsKey(localCall.Identifier))
                        throw new VariableNotFoundException(localCall.Identifier);
                    localCall.Type = ((VariableDeclaration) cls.Members[localCall.Identifier]).Expression.ReturnType;
                }
                else
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
            var newName = $"{localCall.Identifier}$" +
                          $"{localCall.Arguments.Aggregate("", (s, exp) => s += exp.ReturnType)}";
            var method =
                GetMethod(((Class) parent).SelfClassName.Identifier, newName) as MethodDeclaration;
            localCall.Type = method?.ResultType?.Identifier;
            localCall.Identifier = newName;
        }

        public override void Visit(FieldCall field)
        {
            if (!VariableDeclarationChecker.IsDeclared(field, field.Identifier))
                throw new ClassMemberNotFoundException(field.InputType, field.Identifier);
        }

        //TODO check method call
//        public override void Visit(Call call)
//        {
////            var method = GetMethod(call.InputType, call.Identifier);
////            var newName = $"{call.Identifier}$" +
////                          $"{call.Arguments.Aggregate("", (s, exp) => s += exp.ReturnType)}";
////            Console.WriteLine(newName);
//        }

        public override void Visit(Expression expression)
        {
            base.Visit(expression);
//            expression.PrimaryPart.Accept(this);
            var inputType = expression.PrimaryPart.Type;
            expression.ReturnType = inputType;
            for (var i = 0; i < expression.Calls.Count; i++)
            {
                var call = expression.Calls[i];
                call.InputType = inputType;

                var newName = call.Identifier;
                switch (call)
                {
                    case Call c:
                        newName = $"{c.Identifier}$" +
                                  $"{c.Arguments.Aggregate("", (s, exp) => s += exp.ReturnType)}";
                        break;
                    case FieldCall fc:
                        var @class = StaticTables.ClassTable[inputType ?? throw new NullReferenceException()][0];
                        if (@class.NameMap.ContainsKey(fc.Identifier))
                            newName = @class.NameMap[fc.Identifier];
                        break;
                }
                switch (call.InputType)
                {
                    case "Integer":
                    case "Boolean":
                    case "Real":
                        break;
                    default:
                        call.Identifier = newName;
                        break;
                }
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
                        expression.ReturnType = inputType;
                        break;
                }
                if (!(call is Call)) continue;
            }
        }

        public static IMemberDeclaration GetMethod(string className, string identifier)
        {
            if (!StaticTables.ClassTable.ContainsKey(className))
                throw new ClassNotFoundException(className);
            var @class = StaticTables.ClassTable[className][0];
            check:
            if (!@class.Members.ContainsKey(identifier))
            {
                @class = @class.Base ?? throw new ClassMemberNotFoundException(className, identifier);
                goto check;
            }
            return @class.Members[identifier];
        }
    }
}