using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;

namespace Compiler.FrontendPart.SemanticAnalyzer
{
    // TODO Stages of analizer
    // Fill class table
    // Fill variable table for classes
    // Fill variable table for methods and check initialization of variables
    // Generic inheritance
    // Replace Generic classes with existing
    // Check types (should be expanded)
    
    public class Analizer
    {
        private readonly List<Class> _classList;

        public Analizer(List<Class> classList)
        {
            _classList = classList;
        }

        public List<Class> Analize()
        {
            FillStaticTable();
            FillMethodsTable();
            
            return _classList;
        }

        private void FillMethodsTable()
        {
            throw new System.NotImplementedException();
        }

        private void FillStaticTable()
        {
            throw new System.NotImplementedException();
        }
        
        
    }
}