using Compiler.TreeStructure.Expressions;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class VariableDeclaration: IMemberDeclaration, IBody
    {
        public VariableDeclaration(string identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public string Identifier { get; set; }
        public Expression Expression { get; set; }

        public override string ToString()
        {
            return $"Var: {Identifier}";
        }
    }
}