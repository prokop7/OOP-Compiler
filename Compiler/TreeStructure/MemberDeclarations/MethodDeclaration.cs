using System.Collections.Generic;
using System.Linq;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class MethodDeclaration : IMemberDeclaration
    {
        public MethodDeclaration(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; set; }
        public List<ParameterDeclaration> Parameters { get; set; } = new List<ParameterDeclaration>();
        public string ResulType { get; set; }
        public List<IBody> Body { get; set; } = new List<IBody>();

        public override string ToString() =>
            $"Method: {Identifier} ({Parameters.Aggregate("", (current, p) => current + (p.ToString() + ", "))})" + $"";
    }
}