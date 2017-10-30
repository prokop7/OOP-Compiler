using System.Collections.Generic;
using Compiler.TreeStructure;

namespace Compiler
{
    public static class StaticTables
    {
        public static Dictionary<string, Class> ClassTable { get; set; } = new Dictionary<string, Class>();
        
    }
}