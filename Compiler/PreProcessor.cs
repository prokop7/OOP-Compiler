using System.Collections.Generic;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler
{
    public static class PreProcessor
    {
        public static Class SetupCompiler(string startClass, string startMethod)
        {
            return GenerateClass3();
            Class GenerateClass3()
            {
                var className = new ClassName("Main");
                var mainClass = new Class(className);

                var method = new MethodDeclaration("Main") {Parent = mainClass};
                mainClass.MemberDeclarations.Add(method);
                var varExpression = new Expression(new ClassName(startClass));
                
                var body3 = new VariableDeclaration("entry", varExpression) {Parent = method};
                method.Body.Add(body3);

                var localCall = new LocalCall("entry");
                var methodCallExpression = new Expression(localCall) {Parent = method};
                methodCallExpression.Calls.Add(new Call(startMethod) {Parent = methodCallExpression});
                method.Body.Add(methodCallExpression);

                return mainClass;
            }
        }
    }
}