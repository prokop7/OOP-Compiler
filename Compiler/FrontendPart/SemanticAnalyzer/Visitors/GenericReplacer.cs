using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class GenericReplacer: BaseVisitor
    {
        private readonly Dictionary<string, ClassName> _map;

        public GenericReplacer(Dictionary<string, ClassName> map) => _map = map;


        public override void Visit(ClassName className)
        {
            base.Visit(className);
            if (!_map.ContainsKey(className.Identifier)) return;
            
            var parent = className.Parent;
            switch (parent)
            {
                //TODO add other cases
                case ClassName parentClassName:
                    var i = parentClassName.Specification.IndexOf(className);
                    L.Log($"Replaced {parentClassName}", 4);
                    _map[className.Identifier].Parent = className.Parent;
                    parentClassName.Specification[i] = _map[className.Identifier];
                    break;
                case Expression expression:
                    if (expression.PrimaryPart != className)
                        break;
                    L.Log($"Replaced {expression}", 4);
                    _map[className.Identifier].Parent = className.Parent;
                    expression.PrimaryPart = _map[className.Identifier];
                    break;
            }
        }
    }
}