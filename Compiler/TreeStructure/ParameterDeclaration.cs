using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class ParameterDeclaration: ICommonTreeInterface
    {
        public ParameterDeclaration(string identifier, ClassName type)
        {
            Identifier = identifier;
            Type = type;
        }

        public string Identifier { get; set; } // название параметра
        public ClassName Type { get; set; } // тип параметра
        // Visitor visitor:
        //          visitor - Identifier
        //          Visitor - Type

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
