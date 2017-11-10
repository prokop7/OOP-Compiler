using System.Collections.Generic;
using System.Linq;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class Class : Object
    {
        public Class(ClassName name)
        {
            SelfClassName = name;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ClassName SelfClassName { get; set; } = null;
        public ClassName BaseClassName { get; set; } = null;
        public Class Base { get; set; } = null; // класс от которого наследуется текущий класс
 
        public List<IMemberDeclaration> MemberDeclarations { get; set; } =
            new List<IMemberDeclaration>(); // члены класса: перемененные, методы, декларация конструкции
    }
}