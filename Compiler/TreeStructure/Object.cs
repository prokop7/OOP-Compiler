using System.Collections.Generic;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class Object: ICommonTreeInterface
    {
        public string ClassName { get; set; }

        public Object() => ClassName = "Class";

        public virtual void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"Class: {ClassName}";
        }

        public ICommonTreeInterface Parent { get; set; }
    }
}