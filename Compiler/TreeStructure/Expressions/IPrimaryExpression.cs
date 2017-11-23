namespace Compiler.TreeStructure.Expressions
{
    public interface IPrimaryExpression: ICommonTreeInterface
    {
        string Type { get; set; }
    }
}