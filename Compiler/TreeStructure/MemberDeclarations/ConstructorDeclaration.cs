using System.Collections.Generic;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class ConstructorDeclaration: IMemberDeclaration
    {
        public List<ParameterDeclaration> Parameters { get; set; }
        public List<IBody> Body { get; set; }
    }
}