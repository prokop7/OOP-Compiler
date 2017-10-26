using System.Collections.Generic;

namespace Compiler.TreeStructure
{
    public class Object
    {
        public string ClassName { get; set; }

        public Object() => ClassName = "Class";

        public override string ToString()
        {
            return $"Class: {ClassName}";
        }
    }
}