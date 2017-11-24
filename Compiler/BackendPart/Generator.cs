using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.FrontendPart.SemanticAnalyzer.Visitors;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;
using Microsoft.Win32.SafeHandles;

namespace Compiler.BackendPart
{
    public class Generator
    {
        public class ClassStructure
        {
            public TypeBuilder typeBuilder;
            public Dictionary<string, MethodBuilder> methodBuilders;
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
                var classAttrs = TypeAttributes.Public;
                var typeBuilder = modb.DefineType(cls.SelfClassName.Identifier, classAttrs);
                classes[cls.SelfClassName.Identifier] = new ClassStructure
                {
                    typeBuilder = typeBuilder,
                    methodBuilders = new Dictionary<string, MethodBuilder>(),
                    fieldBuilders = new Dictionary<string, FieldBuilder>()
                };

                var ctorTypes = new Type[0];
                var ctorBuilder =
                    typeBuilder.DefineConstructor(
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

                foreach (var memberDeclaration in cls.MemberDeclarations)
                {
                    switch (memberDeclaration)
                    {
                        case ConstructorDeclaration constructorDeclaration:
                            break;
                        case MethodDeclaration method:
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
                            currentMethod = method;
                            var methodBuilder =
                                classes[cls.SelfClassName.Identifier].methodBuilders[method.Identifier];

                            var il = methodBuilder.GetILGenerator();

                            // Generating method locals
                            foreach (var parameter in method.Parameters)
                                GenerateLocal(il, parameter);

                            // Generating method's bodies
                            IBody last = null;
                            foreach (var body in method.Body)
                            {
                                GenerateStatement(il, body);
                                last = body;
                            }
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
        }

        private void GenerateStatement(ILGenerator il, IBody body)
        {
            switch (body)
            {
                case VariableDeclaration variableDeclaration:
                    break;
                case Assignment assignment:
//                    if ( left is SELECTOR )
//                    {
//                    }
//                    else if ( left is ARRAY_ELEM )
//                    {
//                    }
//                    else // left is just NAME
                {
                    var declaration =
                        VariableDeclarationChecker.GetTypeVariable(assignment, assignment.Identifier);
                    
                    if (declaration.Parent is Class)
                    {
                        if (declaration is VariableDeclaration vDecl)
                        {
                            var fb = classes[currentClass.SelfClassName.Identifier].fieldBuilders[vDecl.Identifier];
//                        if (l.isStatic)
//                        {
//                            generateExpression(il, assignment.expression);
//                            il.Emit(OpCodes.Stsfld, fb);
//                        }
//                        else
                            {
                                il.Emit(OpCodes.Ldarg_0);
                                generateExpression(il, assignment.Expression);
                                il.Emit(OpCodes.Stfld, fb);
                            }
                        }
                    }
                    else if (declaration is ParameterDeclaration pd)
                    {
                        // TODO fix number
                        generateExpression(il, assignment.Expression);
                        il.Emit(OpCodes.Starg, 0);
                    }
                    else
                    {
                        // TODO fix number
                        generateExpression(il, assignment.Expression);
                        il.Emit(OpCodes.Stloc, 0);
                    }
                }
                    return;
                case IfStatement ifStmt:
                    var branchFalse = il.DefineLabel();
//                    GenerateRelation(il, ifStmt.Expression, branchFalse);

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
                    return;
                case ReturnStatement returnStatement:
                    if (returnStatement.Expression != null)
                        generateExpression(il, returnStatement.Expression);
                    il.Emit(OpCodes.Ret);
                    break;
                case WhileLoop whileLoop:
                    break;
            }
        }

        //        private void GenerateRelation(ILGenerator il, Expression expression, Label labelFalse)
        //        {
        //            generateExpression(il, expression);
        //            switch (expression.op)
        //            {
        //                case TokenCode.LESS:
        //                    il.Emit(OpCodes.Bge, labelFalse);
        //                    break;
        //                case TokenCode.GREATER:
        //                    il.Emit(OpCodes.Ble, labelFalse);
        //                    break;
        //                case TokenCode.EQUAL:
        //                    il.Emit(OpCodes.Bne_Un, labelFalse);
        //                    break;
        //                case TokenCode.NOT_EQUAL:
        //                    il.Emit(OpCodes.Beq, labelFalse);
        //                    break;
        //            }
        //        }

        private void generateExpression(ILGenerator il, Expression expression)
        {
            var type = expression.PrimaryPart.Type;
            GenerateFactor(il, expression);

//            generateTerm(il, expression.term);
//            if (!expression.positive) il.Emit(OpCodes.Neg);
//            for (int i = 0; i < expression.terms.Count; i++)
//            {
//                generateTerm(il, expression.terms[i]);
//                switch (expression.ops[i])
//                {
//                    case TokenCode.PLUS:
//                        il.Emit(OpCodes.Add);
//                        break;
//                    case TokenCode.MINUS:
//                        il.Emit(OpCodes.Sub);
//                        break;
//                }
//            }
        }


        private void ExpressInteger(ILGenerator il, ICall call)
        {
            switch (call.InputType)
            {
                case "Minus":
                    break;
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


        private void GenerateLocal(ILGenerator il, ParameterDeclaration field)
        {
            var t = field.Type;
            Type type = null;
            if (t.ClassRef == null)
            {
                //TODO Поддержка array
//                if (t.isArray) type = typeof(int[]);
//                else 
                type = typeof(void);
            }
            else
            {
                type = classes[t.Identifier].typeBuilder;
                //if ( t.isArray ) type = type.MakeArrayType();
            }
            var local = il.DeclareLocal(type);
            if (t.ClassRef == null)
            {
                //if ( t.isArray ) il.Emit(OpCodes.Ldnull);
                /*else*/
                il.Emit(OpCodes.Ldc_I4_0);
            }
            else
                il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Stloc, local);
        }
    }
}