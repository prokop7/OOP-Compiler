using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class ClassName : IPrimaryExpression
    {
        public string Identifier { get; set; } // класс от которого наследуется текущий класс
        public List<ClassName> Specification { get; set; } = new List<ClassName>(); // для дженериков
        public ICommonTreeInterface Parent { get; set; }

        public ClassName(string name)
        {
            Identifier = name;
        }

        public ClassName(ClassName className)
        {
            foreach (var name in className.Specification)
                Specification.Add(new ClassName(name) {Parent = this});
            Identifier = string.Copy(className.Identifier);
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            var generics = Specification.Aggregate("", (s1, name) => s1 += $"{name}, ");
            if (generics.EndsWith(", "))
                generics = generics.Remove(generics.Length - 2);
            return !generics.Equals("") ? $"{Identifier}<{generics}>" : Identifier;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClassName)
            {
                var o = obj as ClassName;
                if (this.Specification.Count == o.Specification.Count)
                {
                    return base.Equals(obj);
                }
            }
            //That's useless if you return base.Equals :D
            return base.Equals(obj);
        }
    }
}