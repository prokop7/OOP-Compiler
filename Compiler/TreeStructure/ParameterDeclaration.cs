using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class ParameterDeclaration: ICommonTreeInterface
    {
        public ParameterDeclaration(string identifier, string type)
        {
            Identifier = identifier;
            Type = type;
        }

        public string Identifier { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Type}: {Identifier}";
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}