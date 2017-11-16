﻿using System.Collections.Generic;
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
        
        public Dictionary<string, IMemberDeclaration> Members = new Dictionary<string, IMemberDeclaration>();

        public ClassName SelfClassName { get; set; }
        public ClassName BaseClassName { get; set; } = null;
        public Class Base { get; set; } = null; // класс от которого наследуется текущий класс
 
        public List<IMemberDeclaration> MemberDeclarations { get; set; } =
            new List<IMemberDeclaration>(); // члены класса: перемененные, методы, декларация конструкции

        public override string ToString()
        {
            return SelfClassName.ToString();
        }
    }
}