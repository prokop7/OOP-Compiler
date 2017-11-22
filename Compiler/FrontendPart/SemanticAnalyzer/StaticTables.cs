using System.Collections.Generic;
using Compiler.TreeStructure;
using Compiler.TreeStructure.MemberDeclarations;

namespace Compiler.FrontendPart.SemanticAnalyzer
{
    public static class StaticTables
    {
        public static Dictionary<string, List<Class>> ClassTable { get; set; } = new Dictionary<string, List<Class>>();
        public static Dictionary<string, List<GenericClass>> GenericClassTable { get; set; } = new Dictionary<string, List<GenericClass>>();       
    }
}