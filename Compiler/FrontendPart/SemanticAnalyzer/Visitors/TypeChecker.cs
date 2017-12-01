using System;
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

	        if (methodDeclaration.ResultType != null)
	        {
		        if (!StaticTables.ClassTable.ContainsKey(methodDeclaration.ResultType.Identifier))
                                    throw new ClassNotFoundException(methodDeclaration.ResultType.Identifier);

		        bool res = false;

		        for (int i = 0; i < methodDeclaration.Body.Count; i++)
		        {

			        if (methodDeclaration.Body[i] is ReturnStatement returnStatement)
			        {
				        res = true;
				        if (i != methodDeclaration.Body.Count-1)
				        {
					        methodDeclaration.Body.RemoveRange(i, methodDeclaration.Body.Count - 1);
				        }
				        
			        }
			        else
			        {
				        res |= CheckBranchForReturn(methodDeclaration.Body[i]);
			        }

		        }

		        if (res == false)
		        {
			        throw new MissingReturnStatementException();
		        }

		        bool CheckBranchForReturn(IBody iBody)
		        {
			        switch (iBody)
			        {
				        case IfStatement ifStatement:
					        bool tempBody = false;
					        for (int i = 0; i < ifStatement.Body.Count; i++)
					        {
						        if (CheckBranchForReturn(ifStatement.Body[i]))
						        {
							        tempBody = true;
							        if (i != ifStatement.Body.Count-1)
							        {
								        ifStatement.Body.RemoveRange(i,
									        ifStatement.Body.Count - 1); 
							        }
							        break;
							        
						        }
						        tempBody = false;
					        }

//						foreach (var i in ifStatement.Body)
//						{
//							tempBody |= CheckBranchForReturn(i);
//							ifStatement.Body.RemoveRange();
//							break;
//						}

					        var tempElseBody = false;
					        for (int i = 0; i < ifStatement.ElseBody.Count; i++)
					        {
						        if (CheckBranchForReturn(ifStatement.ElseBody[i]))
						        {
							        tempElseBody = true;
							        if (i != ifStatement.ElseBody.Count - 1)
							        {
								        ifStatement.ElseBody.RemoveRange(i,
									        ifStatement.ElseBody.Count - 1); 
							        }
							        break;
							       
						        }
						        tempElseBody = false;

					        }
					        return tempBody && tempElseBody;
				        case ReturnStatement returnStatement:
					        return true;
				        default:
					        return false;

			        }
		        }
	        }

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


			if (returnStatement.Expression != null)
			{
				if (!StaticTables.ClassTable.ContainsKey(returnStatement.Expression.ReturnType))
				{
					throw new ClassNotFoundException(returnStatement.Expression.ReturnType);
				}

				bool res = checkParent(returnStatement.Parent);

				if (!res)
				{
					throw new InvalidReturnTypeException(
						$"Expected one type, got {returnStatement.Expression.ReturnType}");
				}

				bool checkParent(ICommonTreeInterface memberDeclaration)
				{
					if (memberDeclaration is MethodDeclaration methodDeclaration)
					{
						if (methodDeclaration.ResultType != null)
						{

							if (!returnStatement.Expression.ReturnType.Equals(methodDeclaration.ResultType.Identifier))
							{
								if (StaticTables.ClassTable[returnStatement.Expression.ReturnType][0].BaseClassName != null)
								{
									if (StaticTables.ClassTable[returnStatement.Expression.ReturnType][0].BaseClassName.Identifier
										.Equals(methodDeclaration.ResultType.Identifier))

									{
										return true;
									}
								}


							}
							return returnStatement.Expression.ReturnType.Equals(methodDeclaration.ResultType.Identifier);

						}
						else
						{
							throw new InvalidReturnParentException("Result is void");
						}

					}
					else
					{
						if (memberDeclaration is IStatement)
						{
							bool tempReturn = false;
							switch (memberDeclaration)
							{
								case IfStatement ifStatement:
									tempReturn |= checkParent(ifStatement.Parent);
									break;
								case WhileLoop whileLoop:
									tempReturn |= checkParent(whileLoop.Parent);
									break;
							}
							return tempReturn;
						}
						else
						{
							return true;
						}
//					return checkParent(memberDeclaration.Parent);
//					if (returnStatement.Parent is IfStatement ifStatement)
//					{	
//					
//						bool checkIfBody = ifStatement.Body.Any(i => i.GetType() == typeof(ReturnStatement));
//						bool checkElseBody = ifStatement.ElseBody.Any(i => i.GetType() == typeof(ReturnStatement));
//					
//						if (checkIfBody & checkElseBody == false)
//						{
//							throw new InvalidReturnParentException();
//						}
//					
//					}					
//				
					}
				}
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