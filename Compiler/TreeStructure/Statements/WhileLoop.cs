using System.Collections.Generic;
using Compiler.TreeStructure.Expressions;

namespace Compiler.TreeStructure.Statements
{
    public class WhileLoop: IStatement
    {
        public Expression Expression { get; set; }
        public List<IBody> Body { get; set; }
    }
}