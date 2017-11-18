using System;
using System.Collections.Generic;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public class ClassName : IPrimaryExpression
    {
        public ClassName(string name)
        {
            Identifier = name;
            Specification = new List<string>();
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string Identifier { get; set; } = null; // класс от которого наследуется текущий класс
        public List<ClassName> Specification { get; set; } = null; // для дженериков

        public static implicit operator ClassName(string s)
        {
            return new ClassName(s);
        }
        public static implicit operator string(ClassName className)
        {
            return className.Identifier;
        }

        public ICommonTreeInterface Parent { get; set; }

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
            return base.Equals(obj);
        }

        public override string ToString()
        {
            if (Specification.Count == 0)
            {
                return Identifier;
            }
            else
            {
                String s = Identifier + ": ";
                foreach (var i in Specification)
                {
                    s += i + ", ";

                }
                return s;
            }
            
        }
    }
}