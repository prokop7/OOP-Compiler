﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class ClassName : IPrimaryExpression
    {
        public string Identifier { get; set; } // название класса
        private List<ClassName> _specification  = new List<ClassName>();

        public List<ClassName> Specification
        {
            get => _specification;
            set
            {
                _specification = value;
                _specification?.ForEach(sp => sp.Parent = this);
            }
        } // для дженериков, названия буков дженериков

        public ICommonTreeInterface Parent { get; set; }
        public string Type { set; get; }

        public Class ClassRef => StaticTables.ClassTable.ContainsKey(Identifier)
            ? StaticTables.ClassTable[Identifier][0]
            : null;

        public int? ArrSize { get; set; } = null;


        public ClassName(string name)
        {
            Identifier = name;
            Type = name;
        }

        public ClassName(string name, List<ClassName> specification) : this(name)
        {
            if (specification == null) return;
            Specification = specification;
        }

        public ClassName(ClassName className)
        {
            foreach (var name in className.Specification)
                Specification.Add(new ClassName(name) {Parent = this});
            Identifier = string.Copy(className.Identifier);
            if (className.Type != null) Type = string.Copy(className.Type);
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            var generics = Specification.Aggregate("", (s1, name) => s1 += $"{name}, ");
            if (generics.EndsWith(", "))
                generics = generics.Remove(generics.Length - 2);
            var str = !generics.Equals("") ? $"{Identifier}<{generics}>" : Identifier;
            str += ArrSize != null ? $"[{ArrSize}]" : "";
            return str;
        }
    }
}