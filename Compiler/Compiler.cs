using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Compiler.BackendPart;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler
{
    public static class Compiler
    {
        public static void Compile(List<Class> classList, string filename, string output)
        {
            var foo = classList[0].MemberDeclarations.Any(member => member is ConstructorDeclaration)
                ? null
                : ((MethodDeclaration) classList[0]
                    .MemberDeclarations.FirstOrDefault(member => member is MethodDeclaration))?.Identifier;
            var startClass = PreProcessor.SetupCompiler(classList[0].SelfClassName.Identifier, foo);
            
            classList.Insert(0, startClass);
            
            var analyzer = new Analizer(classList);
            classList = analyzer.Analize();
            var g = new Generator(classList, filename, output);
            g.GenerateProgram();
        }
    }
}