using System.Collections.Generic;
using System.Linq;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class MethodDeclaration : IMemberDeclaration
    {
        public MethodDeclaration(string identifier) => Identifier = identifier;

        public Dictionary<string, IVariableDeclaration> VariableDeclarations { get; set; } =
            new Dictionary<string, IVariableDeclaration>();

        public string Identifier { get; set; } // название метода

        public List<ParameterDeclaration> Parameters { get; set; } =
            new List<ParameterDeclaration>(); // параметры метода

        public string ResulType { get; set; } // result types
        public List<IBody> Body { get; set; } = new List<IBody>(); // тело

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() =>
            $"Method: {Identifier} ({Parameters.Aggregate("", (current, p) => current + (p + ", "))})";

        public ICommonTreeInterface Parent { get; set; }
    }
}