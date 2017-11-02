using System.Collections.Generic;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;
using Compiler.TreeStructure.Visitors;

namespace Compiler
{
    static class EntryPoint
    {
        static void Main(string[] args)
        {
            var mainClass = new Class("Program");
            StaticTables.ClassTable.Add("Program", mainClass);
            StaticTables.ClassTable.Add("Integer", mainClass);
            StaticTables.ClassTable.Add("Real", mainClass);
            StaticTables.ClassTable.Add("Boolean", mainClass);

            var expA = new Expression(new IntegerLiteral(10));
            var varA = new VariableDeclaration("a", expA);
            mainClass.MemberDeclarations.Add(varA);
            
            var method = new MethodDeclaration("FooBar");
            method.ResulType = "Integer";
            
            var expB = new Expression(new RealLiteral(1.5));
            var varB = new VariableDeclaration("b", expB);
            method.Body.Add(varB);
            var assignment = new Assignment("b", new Expression(new IntegerLiteral(22)));
            var expIf = new Expression(new BooleanLiteral(true));
            
            var ifBody = new List<IBody> {assignment};
//            var ifBody = new List<IBody>();
//            ifBody.Add(assignment);
            
            
//            var ifStatement = new IfStatement(expIf, ifBody);
            var ifStatement = new IfStatement(expIf, ifBody, ifBody);
            method.Body.Add(ifStatement);
            
            mainClass.MemberDeclarations.Add(method);


            CheckTypes(mainClass);
        }

        private static void CheckTypes(Class mainClass)
        {
            var typeVisitor = new TypeChecker();
            mainClass.Accept(typeVisitor);
        }
    }
}