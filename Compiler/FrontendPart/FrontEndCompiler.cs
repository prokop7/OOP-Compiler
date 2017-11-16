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
        private string fileName;

        public FrontEndCompiler(string fileName = "program.o")
        {
            this.fileName = fileName;
            lexer = new Lexer(fileName);
        }

        public void Process()
        {
            Token token;
            do
            {
                token = lexer.GetNextToken();
                Console.Write(token + " ");
            } while (token != null);
            
            parser = new Parser(lexer);
            
//            var tree = parser.Analyze();
            
        }
    }
}