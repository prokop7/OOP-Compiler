using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Compiler.FrontendPart.LexicalAnalyzer;
using Compiler.FrontendPart.SyntacticalAnalyzer;

namespace Compiler.FrontendPart
{
    public class FrontEndCompiler
    {
        private Lexer lexer;
        private Parser parser;


        public FrontEndCompiler(string fileName = "program.o")
        {
            lexer = new Lexer(fileName);
            parser = new Parser(lexer);
        }

        public void Process()
        {
            var tokens = lexer.Analyze();
            Console.WriteLine(tokens);
            foreach (Token t in tokens)
            {
                Console.Write(t + " ");
            }
        }
    }
}