using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.Expressions
{
    public class FieldCall: ICall
    {
        public string InputType { get; set; }
        public IMemberDeclaration MemberDeclaration { get; set; }
        public string Identifier { get; set; }
        public ICommonTreeInterface Parent { get; set; }

        public FieldCall(string identifier)
        {
            Identifier = identifier;
        }

        public FieldCall(FieldCall fieldCall)
        {
            Identifier = string.Copy(fieldCall.Identifier);
            if (fieldCall.InputType != null) InputType = string.Copy(fieldCall.InputType);
            switch (fieldCall.MemberDeclaration)
            {
                case ConstructorDeclaration constructorDeclaration:
                    MemberDeclaration = new ConstructorDeclaration(constructorDeclaration);
                    break;
                case MethodDeclaration methodDeclaration:
                    MemberDeclaration = new MethodDeclaration(methodDeclaration);
                    break;
                case VariableDeclaration variableDeclaration:
                    MemberDeclaration = new VariableDeclaration(variableDeclaration);
                    break;
            }
        }
        
        

        public override string ToString()
        {
            return Identifier;
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
        public override string ToString() => Identifier;
    }
}