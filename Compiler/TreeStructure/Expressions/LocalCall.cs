using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class LocalCall: IPrimaryExpression
    {
        public ICommonTreeInterface Parent { get; set; }
        public string Identifier { get; set; }
        public string Type { get; set; }

        public LocalCall(string identifier)
        {
            Identifier = identifier;
        }


        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}