﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.FrontendPart.SemanticAnalyzer.Visitors;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;
using Microsoft.Win32.SafeHandles;
using static Compiler.L;

namespace Compiler.BackendPart
{
    public class Generator
    {
        public class ClassStructure
        {
            public TypeBuilder typeBuilder;
            public Dictionary<string, MethodBuilder> methodBuilders;
            public Dictionary<string, List<LocalBuilder>> locals;
            public ConstructorBuilder ctorBuilder;
            public Dictionary<string, FieldBuilder> fieldBuilders;
        }

        private readonly Dictionary<string, ClassStructure> classes = new Dictionary<string, ClassStructure>();
        private List<Class> _classList;
        private Class currentClass;
        private MethodDeclaration currentMethod;

        public Generator(List<Class> classList)
        {
            _classList = classList;
        }

        public void GenerateProgram()
        {
            Log("Code generating: start", 1);
            var an = new AssemblyName {Name = Path.GetFileNameWithoutExtension("test generator")};
            var ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            var modb = ab.DefineDynamicModule(an.Name, an.Name + ".exe", true);

            modb.CreateGlobalFunctions();


            //            ILGenerator ctorIL = ctorBuilder.GetILGenerator();
            //            ctorIL.Emit(OpCodes.Ldarg_0);
            //
            //            Type[] ctorArgs = new Type[0];
            //            ConstructorInfo ctor = typeof(object).GetConstructor(ctorArgs);
            //            ctorIL.Emit(OpCodes.Call, ctor);
            //            ctorIL.Emit(OpCodes.Ret);
            foreach (var cls in _classList)
            {
                Log($"Creating class {cls}", 3);
                var classAttrs = TypeAttributes.Public;
                var typeBuilder = modb.DefineType(cls.SelfClassName.Identifier, classAttrs);
                classes[cls.SelfClassName.Identifier] = new ClassStructure
                {
                    typeBuilder = typeBuilder,
                    methodBuilders = new Dictionary<string, MethodBuilder>(),
                    fieldBuilders = new Dictionary<string, FieldBuilder>(),
                    locals = new Dictionary<string, List<LocalBuilder>>()
                };

                var ctorTypes = new Type[0];
                var ctorBuilder = typeBuilder.DefineConstructor(
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.Standard,
                    ctorTypes);
                classes[cls.SelfClassName.Identifier].ctorBuilder = ctorBuilder;

                // IL_0000:  ldarg.0
                // IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
                // IL_0006:  ret

                var ctorIl = ctorBuilder.GetILGenerator();
                ctorIl.Emit(OpCodes.Ldarg_0);

                var ctorArgs = new Type[0];
                var ctor = typeof(object).GetConstructor(ctorArgs);
                ctorIl.Emit(OpCodes.Call, ctor);
                ctorIl.Emit(OpCodes.Ret);
            }

            foreach (var cls in _classList)
            {
                var typeBuilder = classes[cls.SelfClassName.Identifier].typeBuilder;
                foreach (var memberDeclaration in cls.MemberDeclarations)
                {
                    switch (memberDeclaration)
                    {
                        case ConstructorDeclaration constructorDeclaration:
                            break;
                        case MethodDeclaration method:
                            Log($"Creating method {method}", 4);
                            var methodAttrs = MethodAttributes.Public;
                            if (method.Identifier == "Main") methodAttrs |= MethodAttributes.Static;

                            Type resType;
                            if (method.ResultType == null) resType = typeof(void);
                            else
                            {
                                // TODO real method type
                                var t = method.ResultType;
                                resType = t.ClassRef == null
                                    ? typeof(void)
                                    : classes[method.ResultType.Identifier].typeBuilder;
                            }
                            var parTypes = new Type[method.Parameters.Count];
                            var i = 0;
                            foreach (var par in method.Parameters)
                            {
                                var t = par.Type;
                                Type parType;
                                parType = t.ClassRef == null ? typeof(int) : classes[t.Identifier].typeBuilder;
                                parTypes[i] = parType;
                                i++;
                            }
                            var mb = typeBuilder.DefineMethod(method.Identifier, methodAttrs, resType, parTypes);
                            classes[cls.SelfClassName.Identifier].methodBuilders.Add(method.Identifier, mb);
                            break;
                        case VariableDeclaration variableDeclaration:
                            Log($"Creating field {variableDeclaration}", 4);
                            var resultType = variableDeclaration.Expression.ReturnType;
                            Type type = null;
                            if (resultType == null)
                            {
//                                if (resulType.isArray) type = typeof(int[]);
//                                else 
                                type = typeof(void);
                            }
                            else
                            {
                                if (resultType == "Boolean")
                                    type = typeof(bool);
                                else
                                    type = classes[resultType].typeBuilder;

//                                if (resultType.isArray) type = type.MakeArrayType();
                            }
                            var attrs = FieldAttributes.Public | FieldAttributes.Static;
                            var fb = typeBuilder.DefineField(variableDeclaration.Identifier, type, attrs);
                            classes[cls.SelfClassName.Identifier].fieldBuilders.Add(variableDeclaration.Identifier, fb);
                            break;
                    }
                }
                // Generating class bodies
            }

            foreach (var cls in _classList)
            {
                var typeBuilder = classes[cls.SelfClassName.Identifier].typeBuilder;
                currentClass = cls;

                // Generating bodies of class methods
                foreach (var memberDeclaration in cls.MemberDeclarations)
                {
                    switch (memberDeclaration)
                    {
                        case ConstructorDeclaration constructorDeclaration:
                            break;
                        case MethodDeclaration method:
                            Log($"Filling method {method}", 5);
                            currentMethod = method;
                            var methodBuilder =
                                classes[cls.SelfClassName.Identifier].methodBuilders[method.Identifier];

                            var il = methodBuilder.GetILGenerator();

                            // Generating method locals
                            foreach (var pair in method.VariableDeclarations)
                                if (pair.Value is VariableDeclaration value)
                                    GenerateLocal(il, value);

                            // Generating method's bodies
                            IBody last = null;
                            foreach (var body in method.Body)
                            {
                                GenerateStatement(il, body);
                                last = body;
                            }
                            PrintAllVariables(il);
                            if (!(last is ReturnStatement) && method.ResultType == null) il.Emit(OpCodes.Ret);

                            // Defining the program entry point
                            if (method.Identifier == "Main")
                                ab.SetEntryPoint(methodBuilder);
                            break;
                        case VariableDeclaration variableDeclaration:
                            break;
                    }
                }
                typeBuilder.CreateType();
            }
            // Saving the assembly
            ab.Save(Path.GetFileName("test generator.exe"));
            Log("Code generating: finish", 1);

            Log($"Output file = {Path.GetFullPath("test generator.exe")}", 0);
        }

        private void GenerateStatement(ILGenerator il, IBody body)
        {
            switch (body)
            {
                case VariableDeclaration variableDeclaration:
                    Log($"Initializate variable {variableDeclaration}", 6);
                    InitLocalVariable(il, variableDeclaration);
                    break;
                case Assignment assignment:
                    Log($"Generate assignment {assignment}", 6);
                    GenerateAssignment(il, assignment);
                    return;
                case IfStatement ifStmt:
                    Log($"Generate if {ifStmt}", 6);
                    GenerateIfStatement(il, ifStmt);
                    return;
                case ReturnStatement returnStatement:
                    if (returnStatement.Expression != null)
                        GenerateExpression(il, returnStatement.Expression);
                    il.Emit(OpCodes.Ret);
                    break;
                case WhileLoop whileLoop:
                    Log($"Generate while {whileLoop}", 6);
                    GenerateWhile(il, whileLoop);
                    break;
            }
        }

        private void InitLocalVariable(ILGenerator il, VariableDeclaration variableDeclaration)
        {
            var i = currentMethod.VariableDeclarations
                .Where(pair => pair.Value is VariableDeclaration)
                .TakeWhile(pair => pair.Key != variableDeclaration.Identifier)
                .Count();
            GenerateExpression(il, variableDeclaration.Expression);
            il.Emit(OpCodes.Stloc, i);
        }

        private void GenerateWhile(ILGenerator il, WhileLoop whileLoop)
        {
            var next = il.DefineLabel();
            var exit = il.DefineLabel();
            il.MarkLabel(next);
            GenerateRelation(il, whileLoop.Expression, exit);
            foreach (var e in whileLoop.Body)
                GenerateStatement(il, e);
            il.Emit(OpCodes.Br, next);
            il.MarkLabel(exit);
        }

        private void GenerateIfStatement(ILGenerator il, IfStatement ifStmt)
        {
            var branchFalse = il.DefineLabel();

            GenerateRelation(il, ifStmt.Expression, branchFalse);

            foreach (var e in ifStmt.Body)
                GenerateStatement(il, e);

            if (ifStmt.ElseBody != null)
            {
                var branchExit = il.DefineLabel();
                il.Emit(OpCodes.Br, branchExit);

                il.MarkLabel(branchFalse);

                foreach (var e in ifStmt.ElseBody)
                    GenerateStatement(il, e);

                il.MarkLabel(branchExit);
            }
            else
                il.MarkLabel(branchFalse);
        }

        private void GenerateRelation(ILGenerator il, Expression ifStmtExpression, Label branchFalse)
        {
            GenerateExpression(il, ifStmtExpression);
            il.Emit(OpCodes.Brfalse, branchFalse);
        }

        /// <summary>
        /// Not tested with fields.
        /// </summary>
        /// <param name="il"></param>
        /// <param name="assignment"></param>
        private void GenerateAssignment(ILGenerator il, Assignment assignment)
        {
            var declaration = VariableDeclarationChecker.GetTypeVariable(assignment, assignment.Identifier);

            if (declaration.Parent is Class)
            {
                // Set values for field
                // TODO change type to Field
                // BUG Doesn't work without static commands
                if (declaration is VariableDeclaration vDecl)
                {
                    var fb = classes[currentClass.SelfClassName.Identifier].fieldBuilders[vDecl.Identifier];
//                    il.Emit(OpCodes.Ldarg_0);
                    GenerateExpression(il, assignment.Expression);
                    il.Emit(OpCodes.Stsfld, fb);
                }
            }
            else if (declaration is ParameterDeclaration pd)
            {
                // TODO fix number
                switch (pd.Parent)
                {
                    case ConstructorDeclaration constructorDeclaration:
                        break;
                    case MethodDeclaration methodDeclaration:
                        var i = methodDeclaration.Parameters.IndexOf(pd, 0);
                        GenerateExpression(il, assignment.Expression);
                        il.Emit(OpCodes.Starg, i);
                        break;
                }
            }
            else
            {
                var i = currentMethod.VariableDeclarations
                    .Where(pair => pair.Value is VariableDeclaration)
                    .TakeWhile(pair => pair.Key != assignment.Identifier)
                    .Count();
                GenerateExpression(il, assignment.Expression);
                il.Emit(OpCodes.Stloc, i);
            }
        }

        private void GenerateExpression(ILGenerator il, Expression expression)
        {
            GeneratePrimaryExpression(il, expression);
            if (expression.Calls.Count > 0)
                GenerateExpressionCalls(il, expression);
        }

        private void GenerateExpressionCalls(ILGenerator il, Expression expression)
        {
            foreach (var expressionCall in expression.Calls)
            {
                switch (expressionCall)
                {
                    case Call call:
                        switch (call.InputType)
                        {
                            case "Boolean":
                                ExpressBoolean(il, call);
                                break;
                            case "Integer":
                                ExpressInteger(il, call);
                                break;
                            case "Real":
                                ExpressReal(il, call);
                                break;
                        }
                        break;
                    case FieldCall fieldCall:
                        break;
                }
            }
        }

        private void ExpressBoolean(ILGenerator il, Call call)
        {
            if (call.Identifier == "Equals")
            {
                GenerateExpression(il, call.Arguments[0]);
                il.Emit(OpCodes.Ceq);
            }
        }

        private void ExpressInteger(ILGenerator il, Call call)
        {
            switch (call.Identifier)
            {
                case "Minus":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Sub);
                    break;
                case "Equals":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "Greater":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Cgt);
                    break;
            }
        }
        
        private void ExpressReal(ILGenerator il, Call call)
        {
            switch (call.Identifier)
            {
                case "Minus":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Sub);
                    break;
                case "Equals":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "Greater":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Cgt);
                    break;
            }
        }

        private void GeneratePrimaryExpression(ILGenerator il, Expression expression)
        {
            var type = expression.PrimaryPart.Type;
            switch (expression.PrimaryPart)
            {
                case LocalCall localCall:
                    var i = currentMethod.VariableDeclarations
                        .Where(pair => pair.Value is VariableDeclaration)
                        .TakeWhile(pair => pair.Key != localCall.Identifier)
                        .Count();
                    il.Emit(OpCodes.Ldloc, i);
                    break;
                case BooleanLiteral booleanLiteral:
                    il.Emit(OpCodes.Ldc_I4, booleanLiteral.Value ? 1 : 0);
                    break;
                case IntegerLiteral integerLiteral:
                    il.Emit(OpCodes.Ldc_I4, integerLiteral.Value);
                    break;
                case RealLiteral realLiteral:
                    il.Emit(OpCodes.Ldc_R8, realLiteral.Value);
                    break;
            }
        }

        private void PrintAllVariables(ILGenerator il)
        {
            var locals = classes[currentClass.SelfClassName.Identifier].locals.ContainsKey(currentMethod.Identifier)
                ? classes[currentClass.SelfClassName.Identifier].locals[currentMethod.Identifier]
                : new List<LocalBuilder>();
            foreach (var local in locals)
            {
                il.EmitWriteLine(local);
            }
        }

        private void GenerateFactor(ILGenerator il, Expression expression)
        {
            switch (expression.PrimaryPart)
            {
                case BooleanLiteral booleanLiteral:
                    il.Emit(OpCodes.Ldc_I4, booleanLiteral.Value ? 1 : 0);
                    break;
                case IntegerLiteral integerLiteral:
                    il.Emit(OpCodes.Ldc_I4, integerLiteral.Value);
                    break;
                case RealLiteral realLiteral:
                    il.Emit(OpCodes.Ldc_R8, realLiteral.Value);
                    break;
            }
        }

        private void GenerateLocal(ILGenerator il, VariableDeclaration variableDeclaration)
        {
            var t = variableDeclaration.Expression.ReturnType;
            Type type = null;
            switch (t)
            {
                case "Boolean":
                    type = typeof(bool);
                    break;
                case "Integer":
                    type = typeof(int);
                    break;
                case "Real":
                    type = typeof(double);
                    break;
                case null:
                    //TODO Поддержка array
//                if (t.isArray) type = typeof(int[]);
//                else 
                    type = typeof(void);
                    break;
                default:
                    type = classes[t].typeBuilder;
                    break;
            }
            var local = il.DeclareLocal(type);
            List<LocalBuilder> locals;
            if (classes[currentClass.SelfClassName.Identifier].locals.ContainsKey(currentMethod.Identifier))
                locals = classes[currentClass.SelfClassName.Identifier].locals[currentMethod.Identifier];
            else
            {
                locals = new List<LocalBuilder>();
                classes[currentClass.SelfClassName.Identifier].locals.Add(currentMethod.Identifier, locals);
            }
            locals.Add(local);
            il.Emit(t == null ? OpCodes.Ldc_I4_0 : OpCodes.Ldnull);
            il.Emit(OpCodes.Stloc, local);
        }
       
    }
}