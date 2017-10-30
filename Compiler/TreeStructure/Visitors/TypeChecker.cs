using System;
using Compiler.Exceptions;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler.TreeStructure.Visitors
{
    public class TypeChecker : IVisitor
    {
        public void Visit(Class @class)
        {
            foreach (var classMemberDeclaration in @class.MemberDeclarations)
            {
                classMemberDeclaration.Accept(this);
            }
        }

        public void Visit(VariableDeclaration variableDeclaration)
        {
            variableDeclaration.Expression.Accept(this);
        }

        public void Visit(Expression expression)
        {
            //TODO check sequantial calls.
            expression.PrimaryPart.Accept(this);
            foreach (var call in expression.Calls)
            {
                call.Accept(this);
            }
            return;
            throw new NotImplementedException();
        }

        public void Visit(RealLiteral realLiteral)
        {
        }

        public void Visit(IntegerLiteral integerLiteral)
        {
        }

        public void Visit(BooleanLiteral booleanLiteral)
        {
        }

        public void Visit(Object obj)
        {
        }

        public void Visit(MethodDeclaration methodDeclaration)
        {
            if (methodDeclaration.ResulType != null &&
                !StaticTables.ClassTable.ContainsKey(methodDeclaration.ResulType))
                throw new ClassNotFoundException(methodDeclaration.ResulType);

            foreach (var parameter in methodDeclaration.Parameters)
                parameter.Accept(this);

            foreach (var element in methodDeclaration.Body)
                element.Accept(this);
        }

        public void Visit(Assignment assignment)
        {
            assignment.Expression.Accept(this);
            // TODO get variable type by idenetifier.
//            if (assignment.Expression.ReturnType != assignment.Identifier)
//                throw new NotValidExpressionTypeException();
            
            
        }

        public void Visit(IfStatement ifStatement)
        {
            ifStatement.Expression.Accept(this);
            if (ifStatement.Expression.ReturnType != "Boolean")
                throw new NotValidExpressionTypeException($"Expected Boolean, got {ifStatement.Expression.ReturnType}");
            foreach (var body in ifStatement.Body)
                body.Accept(this);
            foreach (var elseBody in ifStatement.ElseBody)
                elseBody.Accept(this);
        }

        public void Visit(ReturnStatement returnStatement)
        {
            returnStatement.Expression.Accept(this);
            //TODO check returning type and Expression type
        }

        public void Visit(WhileLoop whileLoop)
        {
            whileLoop.Expression.Accept(this);
            if (whileLoop.Expression.ReturnType != "Boolean")
                throw new NotValidExpressionTypeException($"Expected Boolean, got {whileLoop.Expression.ReturnType}");
        }

        public void Visit(ConstructorDeclaration constructorDeclaration)
        {
            foreach (var parameter in constructorDeclaration.Parameters)
                parameter.Accept(this);
            foreach (var body in constructorDeclaration.Body)
                body.Accept(this);
            
        }

        public void Visit(ParameterDeclaration parameter)
        {
            //TODO get type by identifier
            if (!StaticTables.ClassTable.ContainsKey(parameter.Type))
                throw new ClassNotFoundException();
        }

        public void Visit(MethodOrFieldCall call)
        {
            foreach (var argument in call.Arguments)
            {
                argument.Accept(this);
            }
        }
    }
}