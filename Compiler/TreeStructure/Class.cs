using System.Collections.Generic;
using System.Linq;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class Class: Object
    {
        public Class(string name)
        {
            ClassName = name;
        }
        
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
        
        public Class Base { get; set; } = null;
        public string Specification { get; set; } = null;
        public List<IMemberDeclaration> MemberDeclarations { get; set; } = new List<IMemberDeclaration>();
    }
}