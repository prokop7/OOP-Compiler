using System.Collections.Generic;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class ConstructorDeclaration: IMemberDeclaration
    {
        public List<ParameterDeclaration> Parameters { get; set; }
        public List<IBody> Body { get; set; }
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}