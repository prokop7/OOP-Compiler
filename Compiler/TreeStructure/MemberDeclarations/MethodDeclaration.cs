﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Compiler.TreeStructure.Statements;
using Compiler.TreeStructure.Visitors;

namespace Compiler.TreeStructure.MemberDeclarations
{
    public class MethodDeclaration : IMemberDeclaration
    {
        public MethodDeclaration(string identifier) => Identifier = identifier;

        public MethodDeclaration(MethodDeclaration methodDeclaration)
        {
            Identifier = string.Copy(methodDeclaration.Identifier);
            ResultType = string.Copy(methodDeclaration.ResultType);
            foreach (var parameter in methodDeclaration.Parameters)
                Parameters.Add(new ParameterDeclaration(parameter) {Parent = this});
            foreach (var body in methodDeclaration.Body)
            {
                switch (body)
                {
                    case VariableDeclaration variableDeclaration:
                        Body.Add(new VariableDeclaration(variableDeclaration) {Parent = this});
                        break;
                    case Assignment assignment:
                        Body.Add(new Assignment(assignment) {Parent = this});
                        break;
                    case IfStatement ifStatement:
                        Body.Add(new IfStatement(ifStatement) {Parent = this});
                        break;
                    case ReturnStatement returnStatement:
                        Body.Add(new ReturnStatement(returnStatement) {Parent = this});
                        break;
                    case WhileLoop whileLoop:
                        Body.Add(new WhileLoop(whileLoop) {Parent = this});
                        break;
                }
            }
        }

        public Dictionary<string, IVariableDeclaration> VariableDeclarations { get; set; } =
            new Dictionary<string, IVariableDeclaration>();

        public string Identifier { get; set; } // название метода

        public List<ParameterDeclaration> Parameters { get; set; } =
            new List<ParameterDeclaration>(); // параметры метода

        public string ResultType { get; set; } // result types
        public List<IBody> Body { get; set; } = new List<IBody>(); // тело

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() =>
            $"Method: {Identifier} ({Parameters.Aggregate("", (current, p) => current + (p + ", "))})";

        public ICommonTreeInterface Parent { get; set; }
    }
}