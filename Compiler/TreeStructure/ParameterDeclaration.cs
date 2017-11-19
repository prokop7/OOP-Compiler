using System;
using System.Runtime.Serialization;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class ParameterDeclaration : ICommonTreeInterface, IVariableDeclaration
    {
        public string Identifier { get; set; } // название параметра
        public ICommonTreeInterface Parent { get; set; }

        public ClassName Type { get; set; } // тип параметра
        // Visitor visitor:
        //          visitor - Identifier
        //          Visitor - Type

        public ParameterDeclaration(string identifier, ClassName type)
        {
            Identifier = identifier;
            Type = type;
            type.Parent = this;
        }

        public ParameterDeclaration(ParameterDeclaration parameterDeclaration)
        {
            Identifier = String.Copy(parameterDeclaration.Identifier);
            Type = new ClassName(parameterDeclaration.Type) {Parent = this};
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            return $"{Type}: {Identifier}";
        }
    }
}