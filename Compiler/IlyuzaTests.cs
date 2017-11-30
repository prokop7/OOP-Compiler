using System;
using System.Collections.Generic;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.FrontendPart.SemanticAnalyzer.Visitors;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler
{
    public static class IlyuzaTests
    {

        public static void FillClassStaticTables()
        {
            Console.WriteLine("\nSTART FLAG --------");
            
            
            var analizer = new Analizer(GenerateListOfClasses());
            var retList = analizer.Analize();
  
            PrintList(retList, "Analizer");
            
            List<Class> GenerateListOfClasses()
            {
                var className1 = new ClassName("A");
                //className1.Specification.Add(new ClassName("T") {Parent = className1});
            
                var class1 = new Class(className1);
            
                var className2 = new ClassName("C");
//                className2.Specification.Add(new ClassName("T") {Parent = className2});
//                className2.Specification.Add(new ClassName("F") {Parent = className2});
//            
                var class2 = new Class(className2);
            
            
                var classList = new List<Class> {class1, class2};
                return classList;
            }
            
        }

       
        public static void FillClassMethodTable()
        {
            var methodDeclaration = new MethodDeclaration("Less");          
            methodDeclaration.ResultType = new ClassName("Boolean");
            
            methodDeclaration.Parameters.Add(new ParameterDeclaration("p", new ClassName("Integer")));
            
            var className = new ClassName("B"); 
            var class1 = new Class(className);
            
            class1.MemberDeclarations.Add(methodDeclaration);
            
            var classList = new List<Class> {class1};

            var analizer = new Analizer(classList);
            
            var classIntName = new ClassName("n");
            
            var classInt = BuiltInClasses.GenerateInteger();
            
            

            PrintList(class1.MemberDeclarations, "Class1 Member Declarations");
            PrintList(classInt.MemberDeclarations, "ClassInt Member Declarations");
            
//            var retList = analizer.Analize();
//            foreach (var i in retList)
//            {
//                foreach (var j in i.MemberDeclarations)
//                {
//                    Console.WriteLine(j);
//
//                }
//            }
           
        }

        public static void CheckReturnTypeVisitor()
        {
            var methodDeclaration = new MethodDeclaration("plusN");
            methodDeclaration.Parameters.Add(new ParameterDeclaration("p", new ClassName("Integer")));
            methodDeclaration.ResultType = new ClassName("Integer");
            
            // Integer expression
            var expA = new Expression(new IntegerLiteral(10));
            string sA = expA.ReturnType;
            Console.WriteLine(sA);
            
            // Boolean expression
            var expB = new Expression(new BooleanLiteral(true));
            string sB = expB.ReturnType;
            Console.WriteLine(sB);
            
            var variableDeclaration = new VariableDeclaration("a", expB);
            methodDeclaration.Body.Add(variableDeclaration);
            
            ReturnStatement returnStatement = new ReturnStatement(expA);
            
            methodDeclaration.Body.Add(returnStatement);
//            methodDeclaration.Body[1];
//            foreach (var i in methodDeclaration.Body)
//            {
//                Console.WriteLine(i.);
//            }
//            
            PrintList(methodDeclaration.Body, "Method Declaration");
            
            TypeChecker typeChecker = new TypeChecker();
            
            methodDeclaration.Accept(typeChecker);
            
            
        }
        public static void PrintList<T>(IEnumerable<T> list, string message)
        {
            Console.WriteLine($"#### I START PRINTING {message} ####");
            foreach (var i in list)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine("################");
            
        }

    }
}