using System.Collections.Generic;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class GenericReplacer: BaseVisitor
    {
        private readonly List<string> _identifiers;

        public GenericReplacer(List<string> identifiers) => _identifiers = identifiers;

        public List<ClassName> ReplaceList { get; set; } = new List<ClassName>();

        public override void Visit(Expression expression)
        {
            base.Visit(expression);
            if (expression.PrimaryPart is ClassName className)
            {
                if (className.Specification.Count == 0 && _identifiers.Contains(className.Identifier))
                {
                    className.Identifier = "1234";
                    ReplaceList.Add((ClassName) expression.PrimaryPart);
                }
            }
        }
    }
}