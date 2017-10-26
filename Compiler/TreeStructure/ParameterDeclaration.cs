namespace Compiler.TreeStructure
{
    public class ParameterDeclaration
    {
        public ParameterDeclaration(string identifier, string type)
        {
            Identifier = identifier;
            Type = type;
        }

        public string Identifier { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Type}: {Identifier}";
        }
    }
}