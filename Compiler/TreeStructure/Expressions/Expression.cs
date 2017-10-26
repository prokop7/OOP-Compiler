namespace Compiler.TreeStructure.Expressions
{
    public class Expression
    {
        public Expression(int value)
        {
            Value = value;
        }

        // TODO remove after test
        public int Value { get; set; }
        // TODO Ask and then fill in.

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}