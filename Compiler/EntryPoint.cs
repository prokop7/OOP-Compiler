using System;
using System.Collections.Generic;
using System.IO;
using Compiler.BackendPart;
using Compiler.FrontendPart;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.TreeStructure;


namespace Compiler
{
    static class EntryPoint
    {
        static void Main(string[] args)
        {
            L.LogLevel = 100;
            try
            {
//                AntonTests.VariableDeclaration();
//                AntonTests.GenericClassSetup();
                AntonTests.SimpleClassesTest();
//                AntonTests.BranchTest();
//                AntonTests.WhileTest();
//                AntonTests.IntegerTest();
                
                IlyuzaTests();
                //Compiler.IlyuzaTests.FillClassMethodTable();
                
//                IlyuzaTests.FillClassMethodTable();
                Compiler.IlyuzaTests.FillClassStaticTables();
                Compiler.IlyuzaTests.FillClassMethodTable();
//                CheckTests("Valid");
//                CheckTests("Not Valid");
//                CheckTests("Composite");
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                L.LogError(e);
            }
        }

        private static void IlyuzaTests()
        {
            Console.WriteLine("\nSTART FLAG --------");
            
            var className1 = new ClassName("A");
            className1.Specification.Add(new ClassName("T") {Parent = className1});
            
            var class1 = new Class(className1);
            
            var className2 = new ClassName("A");
            className2.Specification.Add(new ClassName("T") {Parent = className2});
            className2.Specification.Add(new ClassName("F") {Parent = className2});
            
            var class2 = new Class(className2);
            
            var className3 = new ClassName("A");
            className3.Specification.Add(new ClassName("G") {Parent = className3});
            className3.Specification.Add(new ClassName("H") {Parent = className3});
            
            var class3 = new Class(className3);
            

            var classList = new List<Class> {class1, class2, class3};

            var analizer = new Analizer(classList);
            var retList = analizer.Analize();
  
            foreach (var i in retList)
            {
                Console.WriteLine(i);
            }
        }

        private static void CheckTests(string folderName)
        {
            var files = Directory.GetFiles($"./Tests/{folderName}/");
            foreach(var file in files){
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