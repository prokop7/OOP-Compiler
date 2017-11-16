using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure
{
    public interface ICommonTreeInterface: IAcceptVisitor
    {
        ICommonTreeInterface Parent { get; set; }
    }
}