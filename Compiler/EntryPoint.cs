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
//                AntonTests.GenericClassSetup();
                IlyuzaTests.FillClassSaticTables();
                IlyuzaTests.FillClassMethodTable();
//                CheckTests("Valid");
//                CheckTests("Not Valid");
//                CheckTests("Composite");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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