using Compiler.TreeStructure.MemberDeclarations;

namespace Compiler.TreeStructure.Expressions
{
    public interface ICall: ICommonTreeInterface
    {
        string InputType { get; set; }
        IMemberDeclaration MemberDeclaration { get; set; }
        string Identifier { get; set; }
    }
}