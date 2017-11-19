using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace Compiler.TreeStructure
{
    public class GenericClass : Class
    {
        public GenericClass(ClassName name) : base(name)
        {
            SelfClassName = name;
            name.Parent = this;
        }

        public GenericClass(Class @class) : base(@class)
        {
        }

        public GenericClass(GenericClass @class) : base(@class)
        {
            GenericParams = @class.GenericParams;
        }

        public List<string> GenericParams { get; set; } = new List<string>();

        public Dictionary<ClassName, List<ICommonTreeInterface>> References { get; set; } =
            new Dictionary<ClassName, List<ICommonTreeInterface>>();
    }
}