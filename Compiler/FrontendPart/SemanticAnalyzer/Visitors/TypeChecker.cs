﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
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

			bool res = false;

			for (int i = 0; i < methodDeclaration.Body.Count; i++)
			{
				
				if (methodDeclaration.Body[i] is ReturnStatement returnStatement)
				{
					res = true;
					methodDeclaration.Body.RemoveRange(i, methodDeclaration.Body.Count-1);
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
								ifStatement.Body.RemoveRange(i, ifStatement.Body.Count-1); // будет ли он ругаться на последний элемент?
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
								ifStatement.ElseBody.RemoveRange(i, ifStatement.ElseBody.Count-1); // будет ли он ругаться на последний элемент?
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
		
		public override void Visit(Assignment assignment)
		{
			base.Visit(assignment);
			 //TODO get variable type by idenetifier. - Done
			// Дстать переменную по идентификатору - look at variable declaration
			
//            if (assignment.Expression.ReturnType != assignment.Identifier)
//                throw new NotValidExpressionTypeException();
			if (!(assignment.Parent.GetType().Equals(assignment.Expression.ReturnType)))
			{
				throw new NotValidExpressionTypeException();
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

			
			bool res = checkParent(returnStatement.Parent);
			Console.WriteLine($"Result of return statement checking - {res}");

			Console.WriteLine(returnStatement.Expression.ReturnType + " - this is return type");

			if (!res)
			{
				throw new InvalidReturnTypeException(
                							$"Expected one type, got {returnStatement.Expression.ReturnType}");
			}

			bool checkParent(ICommonTreeInterface memberDeclaration)
			{
				if (memberDeclaration is MethodDeclaration methodDeclaration)
				{

					if (returnStatement.Expression.ReturnType.Equals(methodDeclaration.ResultType.Identifier))
					{
						return true;
					}

					return false;
				}
				else
				{
					if (memberDeclaration is IStatement)
					{
						switch (memberDeclaration)
						{
							case IfStatement ifStatement:
								checkParent(ifStatement.Parent);
								break;
							case WhileLoop whileLoop:
								checkParent(whileLoop.Parent);
								break;
						}
						return false;
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