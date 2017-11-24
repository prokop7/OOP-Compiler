﻿using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Exceptions;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;
using Compiler.TreeStructure.Visitors;

namespace Compiler.FrontendPart.SemanticAnalyzer.Visitors
{
    public class VariableDeclarationChecker : BaseVisitor
    {
        public List<ICommonTreeInterface> Stack { get; set; } = new List<ICommonTreeInterface>();

        public int VariableNum { get; set; }

        public string GetContextIdentifier(string identifier)
        {
            var newIdentifier = string.Copy(identifier) + VariableNum;
            VariableNum++;
            return $"${newIdentifier}";
        }

        public void SetMap(string identifier, string newIdentifier)
        {
            switch (Stack.Last())
            {
                case Class el:
                    el.NameMap.Add(identifier, newIdentifier);
                    break;
                case ConstructorDeclaration el:
                    el.NameMap.Add(identifier, newIdentifier);
                    break;
                case MethodDeclaration el:
                    el.NameMap.Add(identifier, newIdentifier);
                    break;
                case IfStatement el:
                    el.NameMap.Add(identifier, newIdentifier);
                    break;
                case WhileLoop el:
                    el.NameMap.Add(identifier, newIdentifier);
                    break;
            }
        }

        public override void Visit(Assignment assignment)
        {
            base.Visit(assignment);
            if (!HasMap(assignment.Identifier))
                throw new VariableNotFoundException(assignment.Identifier);
            assignment.Identifier = GetValueFromMap(assignment.Identifier);
        }

        public override void Visit(Class @class)
        {
            Stack.Add(@class);
            base.Visit(@class);
            Stack.RemoveAt(Stack.Count - 1);
        }

        public override void Visit(This @this)
        {
            // TODO checking identifier
        }

        public override void Visit(MethodDeclaration methodDeclaration)
        {
            Stack.Add(methodDeclaration);
            base.Visit(methodDeclaration);
            Stack.RemoveAt(Stack.Count - 1);
        }

        public override void Visit(WhileLoop whileLoop)
        {
            Stack.Add(whileLoop);
            base.Visit(whileLoop);
            Stack.RemoveAt(Stack.Count - 1);
        }

        public override void Visit(IfStatement ifStatement)
        {
            Stack.Add(ifStatement);
            base.Visit(ifStatement);
            Stack.RemoveAt(Stack.Count - 1);
        }

        public override void Visit(ConstructorDeclaration constructorDeclaration)
        {
            Stack.Add(constructorDeclaration);
            base.Visit(constructorDeclaration);
            Stack.RemoveAt(Stack.Count - 1);
        }

        #region Variable Declaring logic

        public override void Visit(ParameterDeclaration parameter)
        {
            if (HasMap(parameter.Identifier))
                throw new DuplicatedDeclarationException(parameter.Identifier);
            var newName = GetContextIdentifier(parameter.Identifier);
            SetMap(parameter.Identifier, newName);
            parameter.Identifier = newName;
            switch (Stack[1])
            {
                case ConstructorDeclaration constructorDeclaration:
                    constructorDeclaration.VariableDeclarations.Add(parameter.Identifier, parameter);
                    break;
                case MethodDeclaration methodDeclaration:
                    methodDeclaration.VariableDeclarations.Add(parameter.Identifier, parameter);
                    break;
            }
        }

        public override void Visit(VariableDeclaration variable)
        {
            if (HasMap(variable.Identifier))
                throw new DuplicatedDeclarationException(variable.Identifier);
            var newName = GetContextIdentifier(variable.Identifier);
            SetMap(variable.Identifier, newName);
            variable.Identifier = newName;
            if (variable.Parent is Class @class)
            {
                @class.Members.Add(variable.Identifier, variable);
                return;
            }
            switch (Stack[1])
            {
                case MethodDeclaration method:
                    method.VariableDeclarations.Add(variable.Identifier, variable);
                    break;
                case IfStatement ifStatement:
                    ifStatement.VariableDeclarations.Add(variable.Identifier, variable);
                    break;
                case WhileLoop whileLoop:
                    whileLoop.VariableDeclarations.Add(variable.Identifier, variable);
                    break;
                case ConstructorDeclaration constructorDeclaration:
                    constructorDeclaration.VariableDeclarations.Add(variable.Identifier, variable);
                    break;
            }
        }

        #endregion

        public string GetValueFromMap(string identifier)
        {
            foreach (var commonTreeInterface in Stack)
                switch (commonTreeInterface)
                {
                    case Class el when el.NameMap.ContainsKey(identifier):
                        return el.NameMap[identifier];
                    case ConstructorDeclaration el when el.NameMap.ContainsKey(identifier):
                        return el.NameMap[identifier];
                    case MethodDeclaration el when el.NameMap.ContainsKey(identifier):
                        return el.NameMap[identifier];
                    case IfStatement el when el.NameMap.ContainsKey(identifier):
                        return el.NameMap[identifier];
                    case WhileLoop el when el.NameMap.ContainsKey(identifier):
                        return el.NameMap[identifier];
                }
            return null;
        }

        public bool HasMap(string identifier) => !(GetValueFromMap(identifier) is null);

        public static ICommonTreeInterface GetTypeVariable(ICommonTreeInterface node, string identifier)
        {
            while (true)
            {
                switch (node)
                {
                    case null:
                        return null;
                    case Class @class:
                        return @class.Members.ContainsKey(identifier) ? @class.Members[identifier] : null;
                    case MethodDeclaration method:
                        if (method.VariableDeclarations.ContainsKey(identifier))
                            return method.VariableDeclarations[identifier];
                        break;
                    case IfStatement ifStatement:
                        if (ifStatement.VariableDeclarations.ContainsKey(identifier))
                            return ifStatement.VariableDeclarations[identifier];
                        break;
                    case WhileLoop whileLoop:
                        if (whileLoop.VariableDeclarations.ContainsKey(identifier))
                            return whileLoop.VariableDeclarations[identifier];
                        break;
                    case ConstructorDeclaration constructorDeclaration:
                        if (constructorDeclaration.VariableDeclarations.ContainsKey(identifier))
                            return constructorDeclaration.VariableDeclarations[identifier];
                        break;
                }
                node = node.Parent;
            }
        }

        public static bool IsDeclared(ICommonTreeInterface node, string identifier)
        {
            var res = GetTypeVariable(node, identifier);
            return res != null;
        }
    }
}