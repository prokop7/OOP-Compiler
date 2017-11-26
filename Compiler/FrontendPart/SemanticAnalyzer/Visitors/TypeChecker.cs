﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Compiler.Exceptions;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class TypeChecker : BaseVisitor
    {
        public override void Visit(ConstructorCall constructorCall)
        {
            base.Visit(constructorCall);
            switch (constructorCall.Type)
            {
                case "Real" when constructorCall.Arguments[0].ReturnType == "Integer" ||
                                 constructorCall.Arguments[0].ReturnType == "Boolean":
                    constructorCall.Arguments[0].Calls.Add(new Call("ToReal")
                    {
                        Parent = constructorCall.Arguments[0],
                        InputType = constructorCall.Arguments[0].ReturnType
                    });
                    break;
                case "Integer" when constructorCall.Arguments[0].ReturnType == "Real" ||
                                    constructorCall.Arguments[0].ReturnType == "Boolean":
                    constructorCall.Arguments[0].Calls.Add(new Call("ToInteger")
                    {
                        Parent = constructorCall.Arguments[0],
                        InputType = constructorCall.Arguments[0].ReturnType
                    });
                    break;
            }
        }

        public override void Visit(Expression expression)
        {
            base.Visit(expression);
            if (!(expression.PrimaryPart is ClassName className)) return;

            if (!StaticTables.ClassTable.ContainsKey(className.Identifier))
                throw new ClassNotFoundException(className.Identifier);
            //TODO check sequantial calls.
        }

        public override void Visit(MethodDeclaration methodDeclaration)
        {
            base.Visit(methodDeclaration);
            if (methodDeclaration.ResultType != null &&
                !StaticTables.ClassTable.ContainsKey(methodDeclaration.ResultType.Identifier))
                throw new ClassNotFoundException(methodDeclaration.ResultType.Identifier);
        }

        public override void Visit(Assignment assignment)
        {
            base.Visit(assignment);
            //TODO get variable type by idenetifier. - Done
            // Дстать переменную по идентификатору - look at variable declaration

//            if (assignment.Expression.ReturnType != assignment.Identifier)
//                throw new NotValidExpressionTypeException();
            var declaration = VariableDeclarationChecker.GetTypeVariable(assignment, assignment.Identifier);
            switch (declaration)
            {
                case VariableDeclaration variableDeclaration:
                    if (variableDeclaration.Expression.ReturnType == "Real" &&
                        assignment.Expression.ReturnType == "Integer")
                    {
                        var call = new Call("ToReal")
                        {
                            Parent = assignment.Expression,
                            InputType = "Integer"
                        };
                        assignment.Expression.Calls.Add(call);
                        assignment.Expression.ReturnType = "Real";
                    }
                    if (variableDeclaration.Expression.ReturnType != assignment.Expression.ReturnType)
                        throw new NotValidExpressionTypeException();
                    break;
                case ParameterDeclaration parameterDeclaration:
                    if (parameterDeclaration.Type.Identifier == "Real" &&
                        assignment.Expression.ReturnType == "Integer")
                    {
                        var call = new Call("ToReal")
                        {
                            Parent = assignment.Expression,
                            InputType = "Integer"
                        };
                        assignment.Expression.Calls.Add(call);
                        assignment.Expression.ReturnType = "Real";
                    }
                    if (parameterDeclaration.Type.Identifier != assignment.Expression.ReturnType)
                        throw new NotValidExpressionTypeException();
                    break;
            }
        }

        public override void Visit(IfStatement ifStatement)
        {
            base.Visit(ifStatement);
            if (ifStatement.Expression.ReturnType != "Boolean")
                throw new NotValidExpressionTypeException($"Expected Boolean, got {ifStatement.Expression.ReturnType}");
        }

        public override void Visit(ReturnStatement returnStatement)
        {
            base.Visit(returnStatement);
            //TODO check returning type and Expression type - Done
            if (!(returnStatement.Parent is MethodDeclaration @methodDeclaration))
            {
                // go to parent};
                //@methodDeclaration = returnStatement.Parent;

//				if (!(returnStatement.Expression.ReturnType.Equals(@methodDeclaration.ResultType)))
//				{
//					throw new InvalidReturnType(
//						$"Expected {@methodDeclaration.ResultType}, got {returnStatement.Expression.ReturnType}");
//				}
            }
        }

        public override void Visit(WhileLoop whileLoop)
        {
            base.Visit(whileLoop);
            if (whileLoop.Expression.ReturnType != "Boolean")
                throw new NotValidExpressionTypeException($"Expected Boolean, got {whileLoop.Expression.ReturnType}");
        }

        public override void Visit(ParameterDeclaration parameter)
        {
            base.Visit(parameter);
            //TODO get type by identifier

            if (!StaticTables.ClassTable.ContainsKey(parameter.Type.ToString()))
                throw new ClassNotFoundException();
        }
    }
}