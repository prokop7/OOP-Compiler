using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Compiler.FrontendPart.SemanticAnalyzer.Visitors;
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

        public void VariableDeclarationCheck()
        {
            var visitor = new VariableDeclarationChecker();
            foreach (var @class in _classList)
            {
                visitor.Visit(@class);
            }
            Console.WriteLine("Variable declaration checking is Done");
        }
        
        
    }
}