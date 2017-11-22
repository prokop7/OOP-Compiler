using System.Collections.Generic;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;
using Microsoft.VisualBasic.CompilerServices;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class GenericUsageFinder: BaseVisitor
    {
        public List<ClassName> GenericUsages { get; set; } = new List<ClassName>();

        public override void Visit(ClassName className)
        {
            base.Visit(className);
            if (className.Specification.Count == 0)
                return;
            if (className.Parent is Expression || className.Parent is ParameterDeclaration || className.Parent is ClassName)
                GenericUsages.Add(className);
        }
    }
}