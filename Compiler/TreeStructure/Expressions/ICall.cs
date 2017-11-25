﻿using Compiler.TreeStructure.MemberDeclarations;

namespace Compiler.TreeStructure.Expressions
{
    public interface ICall: ICommonTreeInterface
    {
        string InputType { get; set; }
        string Identifier { get; set; }
        IMemberDeclaration MemberDeclaration { get; set; }
    }
}