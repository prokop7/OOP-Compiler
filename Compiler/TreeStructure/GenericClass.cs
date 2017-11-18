using System.Collections.Generic;
namespace Compiler.TreeStructure
{
    public class GenericClass : Class
    {
        public GenericClass(ClassName name) : base(name)
        {
            SelfClassName = name;
        }

        public Dictionary<ClassName, List<ICommonTreeInterface>> References { get; set; } =
            new Dictionary<ClassName, List<ICommonTreeInterface>>();
    }
}