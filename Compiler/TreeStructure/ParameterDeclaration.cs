using System;
using System.Runtime.Serialization;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class ParameterDeclaration: ICommonTreeInterface, IVariableDeclaration
    {
        public ParameterDeclaration(string identifier, ClassName type)
        {
            Identifier = identifier;
            Type = type;
        }

        public ParameterDeclaration(ParameterDeclaration parameterDeclaration)
        {
            Identifier = String.Copy(parameterDeclaration.Identifier);
            Type = new ClassName(parameterDeclaration.Type) {Parent = this};
            
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

        public ICommonTreeInterface Parent { get; set; }
    }
}
