﻿using System;
using System.Collections.Generic;
using Compiler.FrontendPart.SemanticAnalyzer.Visitors;
using Compiler.TreeStructure;

namespace Compiler.FrontendPart.SemanticAnalyzer
{
    /// TODO Stages of analizer
    /// [??%] Fill class table    <see cref="FillStaticTable"/>
    /// [??%] Fill variable/methods table for classes    <see cref="FillMethodsTable"/>
    /// [90%] Simple inheritance    <see cref="AddInheritedMembers"/>
    /// [80%] Fill variable table for methods and check initialization of variables    <see cref="VariableDeclarationCheck"/>
    /// [ 0%] Replace Generic classes with existing
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
            var usageFinder = new GenericUsageFinder();

            foreach (var @class in _classList)
            {
                if (!(@class is GenericClass))
                    usageFinder.Visit(@class);
                else
                {
                    var c = new GenericClass((GenericClass)@class);
                    var genClass = c;
                    var genericReplacer = new GenericReplacer(genClass.GenericParams);
                    genericReplacer.Visit(c);
                    var places = genericReplacer.ReplaceList;
                }
            }
            var genericUsages = usageFinder.GenericUsages;
            foreach (var usage in genericUsages)
                Console.WriteLine(usage);
            // TODO Add base classes such as Integer, AnyRef and so on
        }

        public List<Class> Analize()
        {
            FillStaticTable();
            FillMethodsTable();
            AddInheritedMembers();
            InitClasses();
            VariableDeclarationCheck();
            CheckMethodDeclaration();
            return _classList;
        }

        private void CheckMethodDeclaration()
        {
            var visitor = new MethodCallsChecker();
        }

        private void FillMethodsTable()
        {
            throw new System.NotImplementedException();
        }

        private void FillStaticTable()
        {
            throw new System.NotImplementedException();
        }

        private void AddInheritedMembers()
        {
            foreach (var @class in _classList)
            {
                AddParentMethods(@class);
            }

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