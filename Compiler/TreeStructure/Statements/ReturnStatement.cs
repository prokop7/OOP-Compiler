using Compiler.TreeStructure.Expressions;

namespace Compiler.TreeStructure.Statements
{
    public class ReturnStatement: IStatement
    {
        public Expression Expression { get; set; }
    }
}