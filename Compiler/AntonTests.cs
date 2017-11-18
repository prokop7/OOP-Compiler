using System;
using System.Collections.Generic;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;

namespace Compiler
{
    public static class AntonTests
    {
        public static void VariableDeclaration()
        {
            var @class = GenerateClass();
            var a = new Analizer(new List<Class> {@class});
            a.VariableDeclarationCheck();

            Class GenerateClass()
            {
                var mainClass = new Class(new ClassName("Program"));
                var method = new MethodDeclaration("FooBar");
                method.Parent = mainClass;
                mainClass.MemberDeclarations.Add(method);


                var expA = new Expression(new IntegerLiteral(10));
                var varA = new VariableDeclaration("a", expA);
                varA.Parent = method;
                method.Body.Add(varA);

                var expB = new Expression(new RealLiteral(1.5));
                var varB = new VariableDeclaration("b", expB);
                varB.Parent = method;
                method.Body.Add(varB);

                var expB2 = new Expression(new RealLiteral(124.1));
                var varB2 = new VariableDeclaration("b", expB2);
                varB2.Parent = mainClass;
                mainClass.MemberDeclarations.Add(varB2);
                mainClass.Members.Add("b", varB2);
                return mainClass;
            }
        }

        public static void GenericClassSetup()
        {
            var class1 = GenerateGenericClass();
            var class2 = GenerateClass();
            var a = new Analizer(new List<Class> {class1, class2});
            a.InitClasses();
            

            Class GenerateGenericClass()
            {
                var className = new ClassName("A");
                var tt = new ClassName("T");
                className.Specification.Add(tt);
                tt.Parent = className;
                var mainClass = new GenericClass(className);
                mainClass.GenericParams.Add("T");
                className.Parent = mainClass;

                var t = new ClassName("T");
                var expB2 = new Expression(t);
                t.Parent = expB2;
                
                var varB2 = new VariableDeclaration("b", expB2) {Parent = mainClass};
                mainClass.MemberDeclarations.Add(varB2);
                mainClass.Members.Add("b", varB2);
                return mainClass;
            }

            Class GenerateClass()
            {
                var bClass = new ClassName("B");
                var mainClass = new Class(bClass);
                bClass.Parent = mainClass;


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
                mainClass.Members.Add("c", varB2);
                return mainClass;
            
            }
        }
    }
}