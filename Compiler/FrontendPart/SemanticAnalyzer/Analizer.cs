using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
//            FillMethodsTable();
            
            return _classList;
        }

        private void FillMethodsTable()
        {
            throw new System.NotImplementedException();
        }


        private void FillStaticTable()
        {
            try
            {
                foreach (var i in _classList)
                {
                    if (StaticTables.ClassTable.ContainsKey(i.SelfClassName))
                    {
                        if (i.SelfClassName.Specification.Count == 0)
                        {
                            throw new DuplicateNameException();
                        }
                        else
                        {
                            if (StaticTables.ClassTable[i.SelfClassName].SelfClassName.Specification.Count != i.SelfClassName.Specification.Count)
                            {
                                StaticTables.ClassTable.Add(i.SelfClassName, i);
                            }
                            else
                            {
                                throw new DuplicateNameException();
//                            
                            }
                        }
                       
                        
                    
                    }
                    StaticTables.ClassTable.Add(i.SelfClassName, i);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            
        }

        public override string ToString()
        {
            String s = "";
            foreach (var i in _classList)
            {
                s += i.ToString();

            }
            return s;
        }
    }
}