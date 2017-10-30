namespace Compiler.TreeStructure.Visitors
{
    public interface IAcceptVisitor
    {
        void Accept(IVisitor visitor);
    }
}