using System;
using System.Collections.Generic;
using Compiler.Exceptions;
using System.Data;
using System.Linq;
using Compiler.FrontendPart.SemanticAnalyzer.Visitors;
using Compiler.TreeStructure;
using Compiler.TreeStructure.MemberDeclarations;
using static Compiler.L;

namespace Compiler.FrontendPart.SemanticAnalyzer
{
    /// TODO Stages of analizer
    /// [95%] Fill class table    <see cref="FillStaticTable"/>
    /// [ 0%] Fill variable/methods table for classes    <see cref="FillMethodsTable"/>
    /// [90%] Simple inheritance    <see cref="AddInheritedMembers"/>
    /// [80%] Fill variable table for methods and check initialization of variables    <see cref="VariableDeclarationCheck"/>
    /// [70%] Replace Generic classes with existing    <see cref="InitClasses"/>
    /// [10%] Check types (should be expanded)
    /// [30%] Check that called method is declared
    public class Analizer
    {
        private readonly List<Class> _classList;

        public Analizer(List<Class> classList)
        {
            _classList = classList;
        }

        public void InitClasses()
        {
            Log($"Initialization generic classes: start", 1);
            var usageFinder = new GenericUsageFinder();

            foreach (var @class in _classList)
                if (!(@class is GenericClass))
                    usageFinder.Visit(@class);
            var genericUsages = usageFinder.GenericUsages;
            foreach (var genericUsage in genericUsages)
            {
                var genericClasses = StaticTables.GenericClassTable.GetValueOrDefault(genericUsage.Identifier);
                if (genericClasses == null)
                    throw new ClassNotFoundException(genericUsage.ToString());
                var mapList = new Dictionary<string, ClassName>();
                foreach (var genericClass in genericClasses)
                {
                    if (genericClass.GenericParams.Count == genericUsage.Specification.Count)
                    {
                        for (var i = 0; i < genericClass.GenericParams.Count; i++)
                            mapList.Add(genericClass.GenericParams[i], new ClassName(genericUsage.Specification[i]));
                        var settedClass = new Class(genericClass);
                        var genericReplacer = new GenericReplacer(mapList);
                        genericReplacer.Visit(settedClass);
                        Log($"Generated generic class {settedClass}", 2);
                        if (StaticTables.ClassTable.ContainsKey(genericUsage.Identifier))
                            StaticTables.ClassTable[genericUsage.Identifier].Add(settedClass);
                        else
                            StaticTables.ClassTable.Add(genericUsage.Identifier, new List<Class> {settedClass});
                    }
                }
            }

            Log($"Initialization of generic classes: finish", 1);
        }

        public List<Class> Analize()
        {
            FillStaticTable();
//            InitClasses();
            FillMethodsTable();
//            AddInheritedMembers();
//            VariableDeclarationCheck();
//            CheckMethodDeclaration();
            return _classList;
        }

        private void CheckMethodDeclaration()
        {
            Log($"Method declaration checking: start", 1);
            var visitor = new MethodCallsChecker();
            Log($"Method declaration checking: finish", 1);
        }

        private void FillMethodsTable()
        {
            Log($"Fill class method tables: start", 1);
            foreach (var i in _classList)
            {
                foreach (var j in i.MemberDeclarations)
                {
                    if (j is MethodDeclaration methodDeclaration)
                    {
                       
//                        if (StaticTables.ClassTable[i.SelfClassName.Identifier].Count == 1)
//                        {
//                            StaticTables.ClassTable[i.SelfClassName.Identifier].ElementAt(0).MemberDeclarations.Add(j);
//                        }
//                        else
//                        {
//                            
//                        }
//                        Console.WriteLine(StaticTables.ClassTable[i.SelfClassName.Identifier]);
                        StaticTables.ClassTable[i.SelfClassName.Identifier].ElementAt(0).ClassMethods.Add(i.SelfClassName.Identifier, new List<MethodDeclaration>{methodDeclaration});
                        Console.WriteLine("Hrrrr");
                    }
                    else
                    {
                        Console.WriteLine("ZHHHHHHHHHHHHH");
                    }
                }
                
            }
            
        }

        private void FillStaticTable()
        {
            Log($"Fill static tables: start", 1);
            foreach (var i in _classList)
                if (i.SelfClassName.Specification.Count != 0)
                {
                    if (StaticTables.GenericClassTable.ContainsKey(i.SelfClassName.Identifier))
                    {
                        if (StaticTables.GenericClassTable[i.SelfClassName.Identifier]
                            .Any(j => i.SelfClassName.Specification.Count == j.SelfClassName.Specification.Count))
                            throw new DuplicatedDeclarationException(i.SelfClassName.ToString());
                    }
                    PutToGenericClassTable(i.SelfClassName.Identifier, i);
                }
                else if (StaticTables.ClassTable.ContainsKey(i.SelfClassName.Identifier))
                    throw new DuplicatedDeclarationException(i.SelfClassName.ToString());
                else
                    PutToClassTable(i.SelfClassName.Identifier, i);


            void PutToClassTable(string key, Class value)
            {
                if (StaticTables.ClassTable.ContainsKey(key))
                    StaticTables.ClassTable[key].Add(value);
                else
                    StaticTables.ClassTable.Add(key, new List<Class> {value});
            }

            void PutToGenericClassTable(string key, Class value)
            {
                if (StaticTables.GenericClassTable.ContainsKey(key))
                    StaticTables.GenericClassTable[key].Add(new GenericClass(value));
                else
                    StaticTables.GenericClassTable.Add(key, new List<GenericClass> {new GenericClass(value)});
            }
            Log($"Fill static tables: finish", 1);
        }

        private void AddInheritedMembers()
        {
            Log($"Inheritance extending: start", 1);
            foreach (var @class in _classList)
                AddParentMethods(@class);

            void AddParentMethods(Class @class)
            {
                if (@class.Base == null)
                    return;

                AddParentMethods(@class);
                var members = @class.Members;
                foreach (var (key, pair) in @class.Base.Members)
                    if (!members.ContainsKey(key))
                        members.Add(key, pair);
            }

            Log($"Inheritance extending: finish", 1);
        }


        public void VariableDeclarationCheck()
        {
            Log($"Variable declaration check: start", 1);
            var visitor = new VariableDeclarationChecker();
            foreach (var @class in _classList)
                visitor.Visit(@class);
            Log($"Variable declaration check: finish", 1);
        }
    }
}