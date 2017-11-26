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
                expB2.Calls.Add(new Call("Bar") {Parent = expB2});
                expB2.Calls.Add(new Call("Foo") {Parent = expB2});
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

            var startClass = PreProcessor.SetupCompiler("C", "Foo");
            var analyzer = new Analizer(new List<Class> {startClass, class1, class2, class3});
            var list = analyzer.Analize();

            var g = new Generator(list);
            g.GenerateProgram();

            Class GenerateClass1()
            {
                var bClass = new ClassName("B");
                var mainClass = new Class(bClass);
                return mainClass;
            }

            Class GenerateClass2()
            {
                var className = new ClassName("A");
                var mainClass = new Class(className);


                var varExpression = new Expression(new ClassName("Integer"));
                var body3 = new VariableDeclaration("b", varExpression) {Parent = mainClass};
                mainClass.MemberDeclarations.Add(body3);
                mainClass.Members.Add("b", body3);
                return mainClass;
            }

            Class GenerateClass3()
            {
                var className = new ClassName("C");
                var mainClass = new Class(className);

                var method = new MethodDeclaration("Foo") {Parent = mainClass};
                mainClass.MemberDeclarations.Add(method);
                mainClass.Members.Add("Foo", method);

                var booleanLiteral = new BooleanLiteral(true);
                var booleanLiteralFalse = new BooleanLiteral(false);
                var expression = new Expression(booleanLiteral);
                var expressionFalse = new Expression(booleanLiteralFalse);

                var varExpression = new Expression(new ClassName("Integer"));
                var body3 = new VariableDeclaration("b", varExpression) {Parent = method};
                method.Body.Add(body3);

                var varExpression2 = new Expression(new ClassName("A"));
                var body4 = new VariableDeclaration("a", varExpression2) {Parent = method};
                method.Body.Add(body4);


                var call = new Expression(new LocalCall("a"));
                call.Calls.Add(new FieldCall("b") {Parent = call});
                var assignment = new Assignment("b", call) {Parent = method};
                method.Body.Add(assignment);

                return mainClass;
            }
        }

        public static void BranchTest()
        {
//            var main = PreProcess.CreateMain();

            var class1 = GenerateClass1();

            var analyzer = new Analizer(new List<Class> {class1});
            var list = analyzer.Analize();

            var g = new Generator(list);
            g.GenerateProgram();


            Class GenerateClass1()
            {
                var bClass = new ClassName("A");
                var mainClass = new Class(bClass);

                var method = new MethodDeclaration("Main") {Parent = mainClass};
                mainClass.MemberDeclarations.Add(method);

                var booleanLiteral = new BooleanLiteral(true);
                var booleanLiteralFalse = new BooleanLiteral(false);
                var expression = new Expression(booleanLiteral);
                var expressionFalse = new Expression(booleanLiteralFalse);
                var integerLiteral = new IntegerLiteral(123);
                var integerExpression = new Expression(integerLiteral);

                var localCall = new LocalCall("Foo") {Parameters = new List<Expression>()};
                localCall.Parameters.Add(new Expression(integerLiteral) {Parent = localCall});
                var localCallExpression = new Expression(localCall);

                var body3 = new VariableDeclaration("a", localCallExpression) {Parent = method};
                method.Body.Add(body3);

                var foo = new MethodDeclaration("Foo")
                {
                    Parent = mainClass,
                    ResultType = new ClassName("Integer")
                };
                foo.Parameters.Add(new ParameterDeclaration("a", new ClassName("Integer")));
                var fooBody = new ReturnStatement(new Expression(new LocalCall("a"))) {Parent = foo};

                foo.Body.Add(fooBody);

                mainClass.MemberDeclarations.Add(foo);
                mainClass.Members.Add("Foo", foo);
                return mainClass;
            }
        }

        public static void WhileTest()
        {
            var class1 = GenerateClass1();

            var analyzer = new Analizer(new List<Class> {class1});
            var list = analyzer.Analize();

            var g = new Generator(list);
            g.GenerateProgram();

            Class GenerateClass1()
            {
                var bClass = new ClassName("A");
                var mainClass = new Class(bClass);

                var method = new MethodDeclaration("Main") {Parent = mainClass};
                mainClass.MemberDeclarations.Add(method);

                var booleanLiteral = new BooleanLiteral(true);
                var booleanLiteralFalse = new BooleanLiteral(false);
                var integerLiteral = new IntegerLiteral(123);
                var integerOne = new IntegerLiteral(1);
                var integerExpression = new Expression(integerLiteral);
                var integerOneExpression = new Expression(integerOne);
                var expression = new Expression(booleanLiteral);
                var expressionFalse = new Expression(booleanLiteralFalse);

                var subExpression = new Expression(new LocalCall("a"));
                subExpression.Calls.Add(new Call("Minus")
                {
                    Arguments = new List<Expression> {new Expression(integerOneExpression) {Parent = subExpression}},
                    Parent = subExpression
                });
                subExpression.Calls.Add(new Call("Greater")
                {
                    Arguments = new List<Expression> {new Expression(integerOneExpression) {Parent = subExpression}},
                    Parent = subExpression
                });

                var body3 = new VariableDeclaration("a", integerExpression) {Parent = method};
                method.Body.Add(body3);

                var whileLoop = new WhileLoop(subExpression, new List<IBody>()) {Parent = method};
                method.Body.Add(whileLoop);

                var body2 = new Assignment("a", new Expression(expression)) {Parent = whileLoop};
                whileLoop.Body.Add(body2);

//                var returnStatement = new ReturnStatement {Parent = whileLoop};
//                whileLoop.Body.Add(returnStatement);

                return mainClass;
            }
        }

        public static void IntegerTest()
        {
            var class1 = GenerateClass1();

            var analyzer = new Analizer(new List<Class> {class1});
            var list = analyzer.Analize();

            var g = new Generator(list);
            g.GenerateProgram();

            Class GenerateClass1()
            {
                var bClass = new ClassName("A");
                var mainClass = new Class(bClass);

                var method = new MethodDeclaration("Main") {Parent = mainClass};
                mainClass.MemberDeclarations.Add(method);

                var booleanLiteral = new BooleanLiteral(true);
                var booleanLiteralFalse = new BooleanLiteral(false);
                var integerLiteral = new IntegerLiteral(123);
                var integerTwelve = new IntegerLiteral(12);
                var integerExpression = new Expression(integerLiteral);
                var integerTwelveExpression = new Expression(integerTwelve);
                var expression = new Expression(booleanLiteral);
                var expressionFalse = new Expression(booleanLiteralFalse);

                var subExpression = new Expression(new LocalCall("a"));
                subExpression.Calls.Add(new Call("Minus")
                {
                    Arguments = new List<Expression> {new Expression(integerTwelveExpression) {Parent = subExpression}},
                    Parent = subExpression
                });

                var body3 = new VariableDeclaration("a", integerExpression) {Parent = method};
                method.Body.Add(body3);

                var body2 = new Assignment("a", subExpression) {Parent = method};
                method.Body.Add(body2);

//                var returnStatement = new ReturnStatement {Parent = whileLoop};
//                whileLoop.Body.Add(returnStatement);

                return mainClass;
            }
        }
    }
}