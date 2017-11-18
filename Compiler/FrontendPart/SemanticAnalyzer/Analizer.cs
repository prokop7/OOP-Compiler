using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            //FillMethodsTable();
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
                    if (i.SelfClassName.Specification.Count != 0)
                    {
                        if (StaticTables.GeneriClassTable.ContainsKey(i.SelfClassName))
                        {
                            if (StaticTables.GeneriClassTable[i.SelfClassName].Any(j => i.SelfClassName.Specification.Count == j.SelfClassName.Specification.Count))
                            {
                                throw new DuplicateNameException();
                            }          
                        }
                        else
                        {
                            PutToGenericClassTable(i.SelfClassName, i);
                        }
                    }
                    else
                    {
                        if (StaticTables.ClassTable.ContainsKey(i.SelfClassName))
                        {
                            throw new DuplicateNameException();
                        }
                        else
                        {
                            PutToClassTable(i.SelfClassName, i);
                        }
                        
                    }
                   
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            void PutToClassTable(string key, Class value)
            {
                if(StaticTables.ClassTable.ContainsKey(key))
                
                    StaticTables.ClassTable[key].Add(value);
                else
                {
                    StaticTables.ClassTable.Add(key, new List<Class> {value});
                }
            }
            
            void PutToGenericClassTable(string key, Class value)
            {
                if(StaticTables.GeneriClassTable.ContainsKey(key))
                
                    StaticTables.GeneriClassTable[key].Add(value);
                else
                {
                    StaticTables.GeneriClassTable.Add(key, new List<Class> {value});
                }
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