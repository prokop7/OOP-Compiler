using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler.TreeStructure.Visitors
{
    public interface IVisitor
    {
        void Visit(Class obj);
        
        void Visit(Assignment assignment);
        void Visit(IfStatement ifStatement);
        void Visit(ReturnStatement returnStatement);
        void Visit(WhileLoop whileLoop);
        void Visit(Expression methodOrFieldCall);

        void Visit(ConstructorDeclaration constructorDeclaration);
        void Visit(MethodDeclaration methodDeclaration);
        void Visit(VariableDeclaration variableDeclaration);
        
        void Visit(RealLiteral realLiteral);
        void Visit(IntegerLiteral integerLiteral);
        void Visit(BooleanLiteral booleanLiteral);
        
        void Visit(Object obj);
        void Visit(ParameterDeclaration parameterDeclaration);
        void Visit(ClassName className);
        
        void Visit(Base @base);
        void Visit(This @this);
        void Visit(Call call);
        void Visit(FieldCall fieldCall);
    }
}