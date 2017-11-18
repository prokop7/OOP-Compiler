using System;
using System.Collections.Generic;
using System.IO;
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
                AntonTests.GenericClassSetup();
                IlyuzaTests();
//                CheckTests("Valid");
//                CheckTests("Not Valid");
//                CheckTests("Composite");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void IlyuzaTests()
        {
            Console.WriteLine("\nSTART FLAG --------");
            
            ClassName className1 = new ClassName("A");
            className1.Specification.Add(new ClassName("T"));
            
            Class class1 = new Class(className1);
            
            ClassName className2 = new ClassName("A");
            className2.Specification.Add(new ClassName("T"));
            className2.Specification.Add(new ClassName("F"));
            
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