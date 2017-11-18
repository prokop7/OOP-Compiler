using System.Collections.Generic;
using Compiler.TreeStructure;

namespace Compiler.FrontendPart.SemanticAnalyzer
{
    public static class StaticTables
    {
        public static Dictionary<string, Class> ClassTable { get; set; } = new Dictionary<string, Class>();
        public static Dictionary<string, GenericClass> GenericClassesTable { get; set; } = new Dictionary<string, GenericClass>();
    }
}