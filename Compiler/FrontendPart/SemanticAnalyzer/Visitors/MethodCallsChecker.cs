using Compiler.Exceptions;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class MethodCallsChecker: BaseVisitor
    {
        public override void Visit(ClassName className)
        {
            base.Visit(className);
            if (!StaticTables.ClassTable.ContainsKey(className.Identifier))
                throw new ClassNotFoundException(className.Identifier);
        }

        public override void Visit(MethodOrFieldCall call)
        {
            base.Visit(call);
            if (!VariableDeclarationChecker.IsDeclared(call, call.Identifier))
                throw new ClassMemberNotFoundException(call.Identifier);
        }
    }
}