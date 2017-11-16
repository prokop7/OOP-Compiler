using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
//            var files = Directory.GetFiles("./Tests/Valid/");
///*            files = files.Concat(Directory.GetFiles("./Tests/Not Valid/")).ToArray();
//            files = files.Concat(Directory.GetFiles("./Tests/Composite/")).ToArray();*/
//            foreach(string file in files){
//                Console.WriteLine("\n\n" + file + "\n");
//                var main = new FrontEndCompiler(file);
//                try
//                {
//                    main.Process();
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("!!!!!\n" + e.Message + "\n!!!!!");
//                }
//            }
//
//            var mainClass = new Class(new ClassName("Program"));
//            StaticTables.ClassTable.Add("Program", mainClass);
//            StaticTables.ClassTable.Add("Integer", mainClass);
//            StaticTables.ClassTable.Add("Real", mainClass);
//            StaticTables.ClassTable.Add("Boolean", mainClass);
//
//            var expA = new Expression(new IntegerLiteral(10));
//            var varA = new VariableDeclaration("a", expA);
//            mainClass.MemberDeclarations.Add(varA);
//            
//            var method = new MethodDeclaration("FooBar");
//            method.ResulType = "Integer";
//            
//            var expB = new Expression(new RealLiteral(1.5));
//            var varB = new VariableDeclaration("b", expB);
//            method.Body.Add(varB);
//            var assignment = new Assignment("b", new Expression(new IntegerLiteral(22)));
//            var expIf = new Expression(new BooleanLiteral(true));
//            
//            var ifBody = new List<IBody> {assignment};
////            var ifBody = new List<IBody>();
////            ifBody.Add(assignment);
//            
//            
////            var ifStatement = new IfStatement(expIf, ifBody);
//            var ifStatement = new IfStatement(expIf, ifBody, ifBody);
//            method.Body.Add(ifStatement);
//            
//            mainClass.MemberDeclarations.Add(method);
//
//
//            CheckTypes(mainClass);
            
            
            /**
             * Test FillStaticTable function work
             */
            
            Console.WriteLine("\nSTAAAAAAAAAART!");
            ClassName className1 = new ClassName("A");
            className1.Specification.Add("T");
            Class class1 = new Class(className1);
            
            
            ClassName className2 = new ClassName("A");
//            className2.Specification.Add("T");
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
            
//            
        }

//        private static void CheckTypes(Class mainClass)
//        {
//            var typeVisitor = new TypeChecker();
//            mainClass.Accept(typeVisitor);
//        }
    }
}