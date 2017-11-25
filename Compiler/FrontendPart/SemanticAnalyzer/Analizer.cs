using System;
using System.Collections.Generic;
using Compiler.Exceptions;
using System.Data;
using System.Linq;
using Compiler.FrontendPart.SemanticAnalyzer.Visitors;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.Statements;
using Compiler.TreeStructure.Visitors;
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
                List<GenericClass> genericClasses = null;
                if (StaticTables.GenericClassTable.ContainsKey(genericUsage.Identifier))
                    genericClasses = StaticTables.GenericClassTable[genericUsage.Identifier];

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

            Log($"Initialization of generic classes: finish", 2);
        }

        public List<Class> Analize()
        {
            FillStaticTable();
//            GenericTypesCheck();
//            InitClasses();
//            FillMethodsTable();
            AddInheritedMembers();
            VariableDeclarationCheck();
            CheckMethodDeclaration();
            foreach (var @class in _classList)
            {
                var fieldVisitor = new FieldChangeVisitor();
                fieldVisitor.Visit(@class);
            }
            TypeCheck();
            return _classList;
        }

        private void GenericTypesCheck()
        {
            Log($"Generic types check: start", 1);
            foreach (var pair in StaticTables.GenericClassTable)
            {
                foreach (var gClass in pair.Value)
                {
                    Log($"Go into {gClass}: start", 4);
                    var visitor = new GenericTypesCheck(gClass);
                    visitor.Visit(gClass);
                    Log($"Go into {gClass}: finish", 4);
                }
            }
            Log($"Generic types check: finish", 2);
        }

        private void TypeCheck()
        {
            Log($"Type checking: start", 1);
            var visitor = new TypeChecker();
            foreach (var @class in _classList)
                visitor.Visit(@class);
            Log($"Type checking: finish", 2);
        }

        private void CheckMethodDeclaration()
        {
            Log($"Method declaration checking: start", 1);
            var visitor = new MethodCallsChecker();
            _classList.ForEach(c => c.Accept(visitor));
            Log($"Method declaration checking: finish", 2);
        }

        private void FillMethodsTable()
        {
            throw new System.NotImplementedException();
        }

        private void FillStaticTable()
        {
            Log($"Fill static tables: start", 1);
            AnalyzeClass(BuiltInClasses.GenerateBoolean());
            AnalyzeClass(BuiltInClasses.GenerateInteger());
            AnalyzeClass(BuiltInClasses.GenerateReal());
            foreach (var i in _classList)
                AnalyzeClass(i);
            
            void AnalyzeClass(Class i)
            {
                if (i.SelfClassName.Specification.Count != 0)
                {
                    if (StaticTables.GenericClassTable.ContainsKey(i.SelfClassName.Identifier))
                    {
                        if (StaticTables.GenericClassTable[i.SelfClassName.Identifier]
                            .Any(j => i.SelfClassName.Specification.Count == j.SelfClassName.Specification.Count))
                            throw new DuplicatedDeclarationException(i.SelfClassName.ToString());
                    }
                    PutToGenericClassTable(i.SelfClassName.Identifier, (GenericClass) i);
                }
                else if (StaticTables.ClassTable.ContainsKey(i.SelfClassName.Identifier))
                    throw new DuplicatedDeclarationException(i.SelfClassName.ToString());
                else
                    PutToClassTable(i.SelfClassName.Identifier, i);

            }

            void PutToClassTable(string key, Class value)
            {
                if (StaticTables.ClassTable.ContainsKey(key))
                    StaticTables.ClassTable[key].Add(value);
                else
                    StaticTables.ClassTable.Add(key, new List<Class> {value});
            }

            void PutToGenericClassTable(string key, GenericClass value)
            {
                if (StaticTables.GenericClassTable.ContainsKey(key))
                    StaticTables.GenericClassTable[key].Add(new GenericClass(value));
                else
                    StaticTables.GenericClassTable.Add(key, new List<GenericClass> {new GenericClass(value)});
            }

            Log($"Fill static tables: finish", 2);
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

                // TODO something strange. Test it!!!
                AddParentMethods(@class.Base);
                var members = @class.Members;
                foreach (var pair in @class.Base.Members)
                    if (!members.ContainsKey(pair.Key))
                        members.Add(pair.Key, pair.Value);
            }

            Log($"Inheritance extending: finish", 2);
        }

        public void VariableDeclarationCheck()
        {
            Log($"Variable declaration check: start", 1);
            var visitor = new VariableDeclarationChecker();
            foreach (var @class in _classList)
            {
                Log($"Go into {@class}: start", 4);
                visitor.Visit(@class);
                Log($"Go into {@class}: finish", 4);
                var fillVariables = new FillVariablesVisitor();
                fillVariables.Visit(@class);
            }
            Log($"Variable declaration check: finish", 2);
        }
    }

    public class FieldChangeVisitor: BaseVisitor
    {
        public override void Visit(FieldCall fieldCall)
        {
            var inputType = fieldCall.InputType;
            var inputClass = StaticTables.ClassTable[inputType][0];
            if (inputClass.NameMap.ContainsKey(fieldCall.Identifier))
                fieldCall.Identifier = inputClass.NameMap[fieldCall.Identifier];
            else
                throw new ClassMemberNotFoundException(inputType, fieldCall.Identifier);
        }
    }

    public class FillVariablesVisitor: BaseVisitor
    {
        public override void Visit(WhileLoop whileLoop)
        {
            base.Visit(whileLoop);
        }
    }
}