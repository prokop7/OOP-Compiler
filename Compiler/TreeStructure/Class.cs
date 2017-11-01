using System.Collections.Generic;
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
        
        public Class Base { get; set; } = null; // класс от которого наследуется текущий класс
        public string Specification { get; set; } = null; // для дженериков
        public List<IMemberDeclaration> MemberDeclarations { get; set; } = new List<IMemberDeclaration>(); // члены класса: перемененные, методы, декларация конструкции    }
}
