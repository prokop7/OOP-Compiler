using Compiler.Exceptions;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler.TreeStructure.Visitors
{
	public class TypeChecker : BaseVisitor
	{
		public override void Visit(Expression expression)
		{
			base.Visit(expression);
			//TODO check sequantial calls.
		}

		public override void Visit(MethodDeclaration methodDeclaration)
		{
			base.Visit(methodDeclaration);
			if (methodDeclaration.ResulType != null &&
			    !StaticTables.ClassTable.ContainsKey(methodDeclaration.ResulType))
				throw new ClassNotFoundException(methodDeclaration.ResulType);
		}

		public override void Visit(Assignment assignment)
		{
			base.Visit(assignment);
			// TODO get variable type by idenetifier.
//            if (assignment.Expression.ReturnType != assignment.Identifier)
//                throw new NotValidExpressionTypeException();
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
			//TODO check returning type and Expression type
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
			if (!StaticTables.ClassTable.ContainsKey(parameter.Type))
				throw new ClassNotFoundException();
		}
	}
}