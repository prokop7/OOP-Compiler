using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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
                //AntonTests();
                IlyuzaTests();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
//            CheckTests("Valid");
//            CheckTests("Not Valid");
//            CheckTests("Composite");
        }
        
        private static void AntonTests()
        {
            var @class = GenerateClass();
            var a = new Analizer(new List<Class> {@class});
            a.VariableDeclarationCheck();
            
            Class GenerateClass()
            {
                var mainClass = new Class(new ClassName("Program"));
                var method = new MethodDeclaration("FooBar");
                method.Parent = mainClass;
                mainClass.MemberDeclarations.Add(method);
            
            
                var expA = new Expression(new IntegerLiteral(10));
                var varA = new VariableDeclaration("a", expA);
                varA.Parent = method;
                method.Body.Add(varA);

                var expB = new Expression(new RealLiteral(1.5));
                var varB = new VariableDeclaration("b", expB);
                varB.Parent = method;
                method.Body.Add(varB);
            
                var expB2 = new Expression(new RealLiteral(124.1));
                var varB2 = new VariableDeclaration("b", expB2);
                varB2.Parent = mainClass;
                mainClass.MemberDeclarations.Add(varB2);
                mainClass.Members.Add("b", varB2);
                return mainClass;
            }
        }

        private static void IlyuzaTests()
        {
            Console.WriteLine("\nSTART FLAG --------");
            
            ClassName className1 = new ClassName("A");
            className1.Specification.Add("T");
            
            Class class1 = new Class(className1);
            
            ClassName className2 = new ClassName("A");
            className2.Specification.Add("T");
            className2.Specification.Add("F");
            
            Class class2 = new Class(className2);
            
            List<Class> classList = new List<Class>();
            classList.Add(class1);
            classList.Add(class2);
            
            Analizer analizer = new Analizer(classList);
            List<Class> retList = new List<Class>();
            retList = analizer.Analize();
  
            foreach (var i in retList)
            {
                Console.WriteLine(i);
            }
        }

        private static void TreeBuildingExample()
        {
            var mainClass = new Class(new ClassName("Program"));

            Build("Program");
            Build("Integer");
            Build("Real");
            Build("Boolean");
            
            void Build(string key)
            {
                if(StaticTables.ClassTable.ContainsKey(key))
                
                    StaticTables.ClassTable[key].Add(mainClass);
                else
                {
                    StaticTables.ClassTable.Add(key, new List<Class> {mainClass});
                }
            }
           
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