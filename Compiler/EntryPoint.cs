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
            if (args.Length > 0)
            {
                var cName = args.Length > 1 ? args[1] : null;
                var mName = args.Length > 2 ? args[2] : null;
                CompileFile(args[0], cName, mName);
            }
            else
                CompileSuite("./../../Tests/Composite");
        }

        private static void CompileFile(string filename, string entryClass=null, string entryMethod=null)
        {
            var main = new FrontEndCompiler(filename);
            try
            {
                Compiler.Compile(main.GetClasses(), filename, entryClass, entryMethod);
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
                CompileFile(filename);
                Console.WriteLine("\n\n");
            }
        }
    }
}