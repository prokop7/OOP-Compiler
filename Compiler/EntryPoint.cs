using System;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            var mainClass = new Class("Main");

            var method = new MethodDeclaration("foobar");
            var par1 = new ParameterDeclaration("b", "int");
            var par2 = new ParameterDeclaration("c", "string");
            method.Parameters.AddRange(new[] {par1, par2});
            var assignment = new Assignment("a", new Expression(5));
            method.Body.Add(assignment);

            mainClass.MemberDeclarations.Add(new VariableDeclaration("a", new Expression(5)));
            mainClass.MemberDeclarations.Add(method);


            Console.WriteLine("Hello World!");
        }
    }
}