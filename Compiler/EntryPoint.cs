#define CONTRACTS_FULL

using System;
using System.Diagnostics.Contracts;
using System.IO;
using Compiler.FrontendPart;


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
//                CheckTests("Valid");
//                CheckTests("Not Valid");
//                CheckTests("Composite");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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