﻿using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class This : IPrimaryExpression
    {
        public ICommonTreeInterface Parent { get; set; }

        public This(This @this)
        {
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}