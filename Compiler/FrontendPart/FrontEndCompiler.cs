﻿using System;
 using System.Collections.Generic;
 using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Compiler.FrontendPart.LexicalAnalyzer;
using Compiler.FrontendPart.SyntacticalAnalyzer;
 using Compiler.TreeStructure;

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
            /*Token token;
            do
            {
                token = lexer.GetNextToken();
                Console.Write(token + " ");
            } while (token != null);*/
            
            parser = new Parser(lexer);
            
            var tree = parser.Analyze();
            var treeString = "";
            tree.ForEach(t => treeString += t);
            Console.WriteLine(treeString);
        }

        public List<Class> GetClasses()
        {
            parser = new Parser(lexer);
            
            var tree = parser.Analyze();
            parser.Close();
            return tree;
        }
    }
}