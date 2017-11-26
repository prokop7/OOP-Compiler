using System;
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
	

			foreach (var i in methodDeclaration.Body)
			{
					res |= CheckBranchForReturn(i);	
			}

//			if (res == false)
//			{
//				throw new MissingReturnStatementException();
//			}
			
			bool CheckBranchForReturn(IBody iBody)
			{
				switch (iBody)
				{
					case IfStatement ifStatement:
						foreach (var i in ifStatement.Body)
						{
							CheckBranchForReturn(i);
						}
						foreach (var i in ifStatement.ElseBody)
						{
							CheckBranchForReturn(i);
						}
						break;
					case WhileLoop whileLoop:
						foreach (var i in whileLoop.Body)
						{
							CheckBranchForReturn(i);
						}
						break;
					case ReturnStatement _:
						return true;
						
//					default:
//						return false;
				}
				return false;
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
                    if (variableDeclaration.Expression.ReturnType != assignment.Expression.ReturnType)
                        throw new NotValidExpressionTypeException();
                    break;
                case ParameterDeclaration parameterDeclaration:
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

			
			bool res = checkParent(returnStatement.Parent);
			Console.WriteLine($"HHHHHHHHHHHHHHHHHHHHH    {res}");

			Console.WriteLine(returnStatement.Expression.ReturnType + "JJJJJJJJJJJJJJJJ");

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