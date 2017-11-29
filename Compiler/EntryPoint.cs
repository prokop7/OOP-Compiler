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
            L.LogLevel = 0;
            if (args.Length > 1)
            {
                var outputName = (args.Length == 2 ? args[1] : args[0].Split('.')[0]) + ".exe";
                CompileFile(args[0], outputName);
            }
            else
                CompileSuite("./../../Tests/Composite");
        }

        private static void CompileFile(string filename, string output)
        {
            var main = new FrontEndCompiler(filename);
            try
            {
                Compiler.Compile(main.GetClasses(), filename, output);
            }
            catch (Exception e)
            {
                L.LogError(e);
            }
            finally
            {
                StaticTables.ClassTable = new Dictionary<string, List<Class>>();
            }
        }

        private static void CompileSuite(string folderName)
        {
            var files = Directory.GetFiles(folderName);
            foreach (var filename in files)
            {
                Console.WriteLine(Path.GetFullPath(filename));
                CompileFile(filename, Path.GetFileNameWithoutExtension(filename) + ".exe");
                Console.WriteLine("\n\n");
            }
        }
    }
}