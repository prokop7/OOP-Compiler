using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Compiler.FrontendPart;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.FrontendPart.SemanticAnalyzer.Visitors;
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
            try
            {
//                AntonTests.VariableDeclaration();
                AntonTests.GenericClassSetup();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
//            CheckTests("Valid");
//            CheckTests("Not Valid");
//            CheckTests("Composite");
        }
        
     

        private static void TreeBuildingExample()
        {
            var mainClass = new Class(new ClassName("Program"));
            StaticTables.ClassTable.Add("Program", mainClass);
            StaticTables.ClassTable.Add("Integer", mainClass);
            StaticTables.ClassTable.Add("Real", mainClass);
            StaticTables.ClassTable.Add("Boolean", mainClass);

            var expA = new Expression(new IntegerLiteral(10));
            var varA = new VariableDeclaration("a", expA);
            mainClass.MemberDeclarations.Add(varA);
            
            var method = new MethodDeclaration("FooBar");
            method.ResultType = "Integer";
            
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
        }

        private static void CheckTests(string folderName)
        {
            var files = Directory.GetFiles($"./Tests/{folderName}/");
            foreach(string file in files){
                Console.WriteLine("\n\n" + file + "\n");
                var main = new FrontEndCompiler(file);
                try
                {
                    main.Process();
                }
                catch (Exception e)
                {
                    Console.WriteLine("!!!!!\n" + e.Message + "\n!!!!!");
                }
            }
        }
    }
}