using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class This : IPrimaryExpression
    {
        public ICommonTreeInterface Parent { get; set; }
        public string Type { get; set; }


        public This(This @this)
        {
        }

        public This()
        {
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            return "this";
        }
    }
}