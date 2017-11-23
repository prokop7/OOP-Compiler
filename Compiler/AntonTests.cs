using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;
using Compiler.BackendPart;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler
{
    public static class AntonTests
    {
        public static void VariableDeclaration()
        {
            var @class = GenerateClass();
            var a = new Analizer(new List<Class> {@class});
            a.Analize();

            Class GenerateClass()
            {
                var mainClass = new Class(new ClassName("Program"));
                var method = new MethodDeclaration("FooBar");
                method.Parent = mainClass;
                mainClass.MemberDeclarations.Add(method);


                var expA = new Expression(new IntegerLiteral(10));
                var varA = new VariableDeclaration("a", expA) {Parent = method};
                method.Body.Add(varA);

                var expB = new Expression(new RealLiteral(1.5));
                var varB = new VariableDeclaration("b", expB) {Parent = method};
                method.Body.Add(varB);

                var expB2 = new Expression(new RealLiteral(124.1));
                var varB2 = new VariableDeclaration("b", expB2) {Parent = mainClass};
                mainClass.MemberDeclarations.Add(varB2);
                mainClass.Members.Add("b", varB2);
                return mainClass;
            }
        }

        public static void GenericClassSetup()
        {
            var class1 = GenerateGenericClass();
//            StaticTables.GenericClassTable.Add("A", new List<GenericClass> {class1});
            var class2 = GenerateClass();
            var a = new Analizer(new List<Class> {class1, class2});
            a.Analize();


            GenericClass GenerateGenericClass()
            {
                var className = new ClassName("A");
                var tt = new ClassName("T");
                className.Specification.Add(tt);
                tt.Parent = className;
                var mainClass = new GenericClass(className);
                mainClass.GenericParams.Add("T");

                className.Parent = mainClass;
                var method = new MethodDeclaration("Foo")
                {
                    ResultType = new ClassName("A"),
                    Parent = mainClass
                };
                mainClass.MemberDeclarations.Add(method);
                className.Parent = mainClass;
                var method2 = new MethodDeclaration("Bar")
                {
                    ResultType = new ClassName("T"),
                    Parent = mainClass
                };
                mainClass.MemberDeclarations.Add(method2);

                var t = new ClassName("A");
                var expB2 = new Expression(t);
                expB2.Calls.Add(new MethodOrFieldCall("Bar") {Parent = expB2});
                expB2.Calls.Add(new MethodOrFieldCall("Foo") {Parent = expB2});
                t.Parent = expB2;

                var varB2 = new VariableDeclaration("b", expB2) {Parent = mainClass};
                mainClass.MemberDeclarations.Add(varB2);
//                mainClass.Members.Add("b", varB2);
                return mainClass;
            }

            Class GenerateClass()
            {
                var bClass = new ClassName("B");
                var mainClass = new Class(bClass);


                var aClassName = new ClassName("A");
                var nestedA = new ClassName("A");
                var integer = new ClassName("Integer");


                aClassName.Specification.Add(nestedA);
                nestedA.Parent = aClassName;

                nestedA.Specification.Add(integer);
                integer.Parent = nestedA;

                var expB2 = new Expression(aClassName);
                aClassName.Parent = expB2;

                var varB2 = new VariableDeclaration("c", expB2) {Parent = mainClass};
                mainClass.MemberDeclarations.Add(varB2);
//                mainClass.Members.Add("c", varB2);
                return mainClass;
            }
        }

        public static void SimpleClassesTest()
        {
            var class1 = GenerateClass1();
            var class2 = GenerateClass2();
            var class3 = GenerateClass3();
            
            
            var analyzer = new Analizer(new List<Class> {class1, class2, class3});
            var list = analyzer.Analize();
            
            var g = new Generator(list);
            g.GenerateProgram();

            Class GenerateClass1()
            {
                var bClass = new ClassName("B");
                var mainClass = new Class(bClass);

//                var aClassName = new ClassName("A");
//                var expB2 = new Expression(aClassName);
//                aClassName.Parent = expB2;

//                var varB2 = new VariableDeclaration("c", expB2) {Parent = mainClass};
//                mainClass.MemberDeclarations.Add(varB2);
                return mainClass;
            }

            Class GenerateClass2()
            {
                var className = new ClassName("A");
                var mainClass = new Class(className);

                return mainClass;
            }

            Class GenerateClass3()
            {
                var className = new ClassName("C");
                var mainClass = new Class(className);

                var method = new MethodDeclaration("Main") {Parent = mainClass};
                method.Parameters.Add(new ParameterDeclaration("variable", new ClassName("A")));
                mainClass.MemberDeclarations.Add(method);
                
                var method2 = new MethodDeclaration("Foo") {Parent = mainClass};
                method2.Parameters.Add(new ParameterDeclaration("variable", new ClassName("B")) {Parent = method2});
                method2.Parameters.Add(new ParameterDeclaration("variable2", new ClassName("A")) {Parent = method2});
                mainClass.MemberDeclarations.Add(method2);
//
//                var booleanLiteral = new BooleanLiteral(true);
//                var whileExpression = new Expression(booleanLiteral);
//                booleanLiteral.Parent = whileExpression;
//
//                var whileLoop = new WhileLoop(whileExpression) {Parent = method};
//
//                var varExpression = new Expression(new ClassName("C"));
//                varExpression.Calls.Add(new MethodOrFieldCall("Foo2") {Parent = varExpression});
//                
//                var body1 = new VariableDeclaration("a", varExpression) {Parent = whileLoop};
//                var body2 = new Assignment("a", new Expression(varExpression)) {Parent = whileLoop};

//                whileLoop.Body.AddRange(new List<IBody> {body1, body2});
                
////                var body3 = new Assignment("a", new Expression(varExpression)) {Parent = method};
                
//                method.Body.Add(whileLoop);
////                method.Body.Add(body3);

                return mainClass;
            }
        }
    }
}