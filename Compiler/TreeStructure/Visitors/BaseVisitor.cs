using System;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler.TreeStructure.Visitors
{
	/// <summary>
	/// Makes all traverse across the tree
	/// </summary>
	// Рекурсивно пробегает по всему дереву, никаких изменений он не делает
    public class BaseVisitor: IVisitor
	{
		public virtual void Visit(Class @class)
        {
            @class.SelfClassName.Accept(this);
            foreach (var classMemberDeclaration in @class.MemberDeclarations)
                classMemberDeclaration.Accept(this);
        }

        public virtual void Visit(VariableDeclaration variableDeclaration)
        {
            variableDeclaration.Expression.Accept(this);
        }

	    public virtual void Visit(Expression expression)
        {
            expression.PrimaryPart.Accept(this);
            foreach (var call in expression.Calls)
                call.Accept(this);
        }

	    public virtual void Visit(RealLiteral realLiteral)
	    {
	    }

	    public virtual void Visit(IntegerLiteral integerLiteral)
        {
        }

        public virtual void Visit(BooleanLiteral booleanLiteral)
        {
        }

        public virtual void Visit(Object obj)
        {
        }

        public virtual void Visit(MethodDeclaration methodDeclaration)
        {
            foreach (var parameter in methodDeclaration.Parameters)
                parameter.Accept(this);

            foreach (var element in methodDeclaration.Body)
                element.Accept(this);
        }

        public virtual void Visit(Assignment assignment)
        {
            assignment.Expression.Accept(this);
        }

        public virtual void Visit(IfStatement ifStatement)
        {
            ifStatement.Expression.Accept(this);
            foreach (var body in ifStatement.Body)
                body.Accept(this);
            foreach (var elseBody in ifStatement.ElseBody)
                elseBody.Accept(this);
        }

        public virtual void Visit(ReturnStatement returnStatement)
        {
            returnStatement.Expression.Accept(this);
        }

        public virtual void Visit(WhileLoop whileLoop)
        {
            whileLoop.Expression.Accept(this);
        }

        public virtual void Visit(ConstructorDeclaration constructorDeclaration)
        {
            foreach (var parameter in constructorDeclaration.Parameters)
                parameter.Accept(this);
            foreach (var body in constructorDeclaration.Body)
                body.Accept(this);
        }

        public virtual void Visit(ParameterDeclaration parameter)
        {
            throw new NotImplementedException();
        }

	    public virtual void Visit(ClassName className)
	    {
		    for (var i = 0; i < className.Specification.Count; i++)
		    {
			    var name = className.Specification[i];
			    name.Accept(this);
		    }
	    }

	    public virtual void Visit(Base @base)
	    {
	        throw new NotImplementedException();
	    }

	    public virtual void Visit(This @this)
	    {
	        throw new NotImplementedException();
	    }

	    public virtual void Visit(MethodOrFieldCall call)
        {
            foreach (var argument in call.Arguments)
                argument.Accept(this);
        }
	}
}
