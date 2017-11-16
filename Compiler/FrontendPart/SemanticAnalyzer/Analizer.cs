using System.Collections.Generic;
using Compiler.TreeStructure;

namespace Compiler.FrontendPart.SemanticAnalyzer
{
    // TODO Stages of analizer
    // Fill class table
    // Fill variable table for classes
    // Fill variable table for methods and check initialization of variables
    // Replace Generic classes with existing
    // Check types (should be expanded)
    
    public class Analizer
    {
        private readonly List<Class> _classes;

        public Analizer(List<Class> classes)
        {
            _classes = classes;
        }
        
        
        
    }
}