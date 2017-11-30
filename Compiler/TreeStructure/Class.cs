﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class Class : Object
    {
        public Dictionary<string, IMemberDeclaration> Members = new Dictionary<string, IMemberDeclaration>();
        public ClassName SelfClassName { get; set; }
        public ClassName BaseClassName { get; set; }
        public Class Base { get; set; } // класс от которого наследуется текущий класс
        public Dictionary<string, string> NameMap { get; set; } = new Dictionary<string, string>();
        private List<IMemberDeclaration> _memberDeclarations = new List<IMemberDeclaration>();

        public List<IMemberDeclaration> MemberDeclarations
        {
            get => _memberDeclarations;
            set
            {
                _memberDeclarations = value;
                _memberDeclarations?.ForEach(member => member.Parent = this);
            }
        } // члены класса: перемененные, методы, декларация конструкции



        public Class(ClassName name)
        {
            SelfClassName = name;
            name.Parent = this;
        }
        
        public Class(ClassName name, ClassName baseClassName) : this(name)
        {
            BaseClassName = baseClassName;
            BaseClassName.Parent = this;
        }
        
        public Class(ClassName name, ClassName baseClassName, List<IMemberDeclaration> memberDeclarations) : this(name)
        {
            if (baseClassName != null)
            {
                BaseClassName = baseClassName;
                BaseClassName.Parent = this;
            }
            MemberDeclarations = memberDeclarations;
        }

        public Class(Class @class)
        {
            SelfClassName = new ClassName(@class.SelfClassName) {Parent = this};
            if (@class.BaseClassName != null)
                BaseClassName = new ClassName(@class.BaseClassName) {Parent = this};
            if (@class.Base != null)
                Base = new Class(@class.Base);
            foreach (var memberDeclaration in @class.MemberDeclarations)
                switch (memberDeclaration)
                {
                    case ConstructorDeclaration constructorDeclaration:
                        var constructor = new ConstructorDeclaration(constructorDeclaration) {Parent = this};
                        MemberDeclarations.Add(constructor);
                        break;
                    case MethodDeclaration methodDeclaration:
                        var method = new MethodDeclaration(methodDeclaration) {Parent = this};
                        MemberDeclarations.Add(method);
                        Members.Add(method.Identifier, method);
                        break;
                    case VariableDeclaration variableDeclaration:
                        var variable = new VariableDeclaration(variableDeclaration) {Parent = this};
                        MemberDeclarations.Add(variable);
                        Members.Add(variable.Identifier, variable);
                        break;
                }
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            var members = "";
            MemberDeclarations.ForEach(m => members += m + "\n");
            return "class " + SelfClassName + (BaseClassName != null ? $" extends {BaseClassName}" : "") + " is\n" + members + "end\n";
        }
    }
}