using System;
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
        private class ClassStructure
        {
            public TypeBuilder TypeBuilder;
            public Dictionary<string, MethodBuilder> MethodBuilders;
            public Dictionary<string, List<LocalBuilder>> Locals;
            public ConstructorBuilder CtorBuilder;
            public Dictionary<string, FieldBuilder> FieldBuilders;
        }

        private readonly Dictionary<string, ClassStructure> classes = new Dictionary<string, ClassStructure>();
        private readonly List<Class> _classList;
        private Class _currentClass;
        private MethodDeclaration _currentMethod;

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

            foreach (var cls in _classList)
            {
                Log($"Creating class {cls}", 3);
                var classAttrs = TypeAttributes.Public;
                var typeBuilder = modb.DefineType(cls.SelfClassName.Identifier, classAttrs);
                classes[cls.SelfClassName.Identifier] = new ClassStructure
                {
                    TypeBuilder = typeBuilder,
                    MethodBuilders = new Dictionary<string, MethodBuilder>(),
                    FieldBuilders = new Dictionary<string, FieldBuilder>(),
                    Locals = new Dictionary<string, List<LocalBuilder>>()
                };

                var ctorTypes = new Type[0];
                var ctorBuilder = typeBuilder.DefineConstructor(
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.Standard,
                    ctorTypes);
                classes[cls.SelfClassName.Identifier].CtorBuilder = ctorBuilder;

                var ctorIl = ctorBuilder.GetILGenerator();
                ctorIl.Emit(OpCodes.Ldarg_0);

                var ctorArgs = new Type[0];
                var ctor = typeof(object).GetConstructor(ctorArgs);
                ctorIl.Emit(OpCodes.Call, ctor ?? throw new NullReferenceException());
                ctorIl.Emit(OpCodes.Ret);
            }

            foreach (var cls in _classList)
            {
                var typeBuilder = classes[cls.SelfClassName.Identifier].TypeBuilder;
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
                                    : GetTypeByClassIdentifier(t.Identifier);
                            }
                            var parTypes = new Type[method.Parameters.Count];
                            var i = 0;
                            foreach (var par in method.Parameters)
                            {
                                var t = par.Type;
                                var parType = t.ClassRef == null
                                    ? typeof(void)
                                    : GetTypeByClassIdentifier(t.Identifier);
                                parTypes[i] = parType;
                                i++;
                            }
                            var mb = typeBuilder.DefineMethod(method.Identifier, methodAttrs, resType, parTypes);
                            classes[cls.SelfClassName.Identifier].MethodBuilders.Add(method.Identifier, mb);
                            break;
                        case VariableDeclaration variableDeclaration:
                            Log($"Creating field {variableDeclaration}", 4);
                            var resultType = variableDeclaration.Expression.ReturnType;
                            var type = resultType == null ? typeof(void) : GetTypeByClassIdentifier(resultType);
                            const FieldAttributes attrs = FieldAttributes.Public;
                            var fb = typeBuilder.DefineField(variableDeclaration.Identifier, type, attrs);
                            classes[cls.SelfClassName.Identifier].FieldBuilders.Add(variableDeclaration.Identifier, fb);
                            break;
                    }
                }
                // Generating class bodies
            }

            foreach (var cls in _classList)
            {
                var typeBuilder = classes[cls.SelfClassName.Identifier].TypeBuilder;
                _currentClass = cls;

                // Generating bodies of class methods
                foreach (var memberDeclaration in cls.MemberDeclarations)
                {
                    switch (memberDeclaration)
                    {
                        case ConstructorDeclaration constructorDeclaration:
                            break;
                        case MethodDeclaration method:
                            Log($"Filling method {method}", 5);
                            _currentMethod = method;
                            var methodBuilder =
                                classes[cls.SelfClassName.Identifier].MethodBuilders[method.Identifier];

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

        private Type GetTypeByClassIdentifier(string identifier)
        {
            switch (identifier)
            {
                case "Integer":
                    return typeof(int);
                case "Boolean":
                    return typeof(bool);
                default:
                    return classes[identifier].TypeBuilder;
            }
        }

        private void GenerateStatement(ILGenerator il, IBody body)
        {
            switch (body)
            {
                case Expression expression:
                    Log($"Generate expression {expression}", 6);
                    GenerateExpression(il, expression);
                    break;
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
            var i = _currentMethod.VariableDeclarations
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
                    var fb = classes[_currentClass.SelfClassName.Identifier].FieldBuilders[vDecl.Identifier];
                    GenerateExpression(il, assignment.Expression);
                    il.Emit(OpCodes.Stsfld, fb);
                }
            }
            else
            {
                GenerateExpression(il, assignment.Expression);
                VariabelByName(il, assignment.Identifier, true);
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
                            default:
                                var method = classes[expressionCall.InputType]
                                    .MethodBuilders[expressionCall.Identifier];
                                il.EmitCall(OpCodes.Callvirt, method, new Type[0]);
                                break;
                        }
                        break;
                    case FieldCall fieldCall:
                        var fb = classes[fieldCall.InputType].FieldBuilders[fieldCall.Identifier];
                        il.Emit(OpCodes.Ldfld, fb);
                        break;
                }
            }
        }

        private void ExpressBoolean(ILGenerator il, Call call)
        {
            switch (call.Identifier)
            {
                case "Equals":
                    GenerateExpression(il, call.Arguments[0]); 
                    il.Emit(OpCodes.Ceq);
                    break;
                case "And" :
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.And);
                    break;
                case "Or" :
                    GenerateExpression(il, call.Arguments[0]); 
                    il.Emit(OpCodes.Or);                      
                    break;                                     
                case "Not" :
                    GenerateExpression(il, call.Arguments[0]); 
                    il.Emit(OpCodes.Not);                      
                    break;                                     
                case "Xor" :
                    GenerateExpression(il, call.Arguments[0]); 
                    il.Emit(OpCodes.Xor);                      
                    break; 
                case "ToInteger" :
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Conv_I4);
                    break;
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
                case "Plus":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Add);
                    break;
                case "Mult":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Mul);
                    break;
                case "Div":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Div_Un);
                    break;
                case "Equals":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "Greater":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Cgt);
                    break;
                case "Less":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Clt);
                    break;
                case "Rem":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Rem);
                    break;
                case "LessEqual":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Cgt);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "GreaterEqual":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Clt);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "ToReal" :                             
                    GenerateExpression(il, call.Arguments[0]); 
                    il.Emit(OpCodes.Conv_R4);                  
                    break;                                     
                case "UnaryMinus" :                             
                    GenerateExpression(il, call.Arguments[0]); 
                    il.Emit(OpCodes.Neg);                  
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
                case "Plus":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Add);
                    break;
                case "Mult":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Mul);
                    break;
                case "Div":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Div_Un);
                    break;
                case "Equals":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "Greater":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Cgt);
                    break;
                case "Less":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Clt);
                    break;
                case "Rem":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Rem);
                    break;
                case "LessEqual":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Cgt);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "GreaterEqual":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Clt);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "ToInteger" :                             
                    GenerateExpression(il, call.Arguments[0]); 
                    il.Emit(OpCodes.Conv_I4);                  
                    break;   
                case "UnaryMinus" :                            
                    GenerateExpression(il, call.Arguments[0]); 
                    il.Emit(OpCodes.Neg);                      
                    break;                                     
            }
        }

        private void GeneratePrimaryExpression(ILGenerator il, Expression expression)
        {
            switch (expression.PrimaryPart)
            {
                case ClassName className:
                    switch (className.Identifier)
                    {
                        case "Integer":
                        case "Boolean":
                            il.Emit(OpCodes.Ldc_I4, 0);
                            break;
                        case "Real":
                            il.Emit(OpCodes.Ldc_R8, 0);
                            break;
                        default:
                            var local = classes[className.Identifier].CtorBuilder;
                            il.Emit(OpCodes.Newobj, local);
                            break;
                    }
                    break;
                case LocalCall localCall:
                    if (localCall.Parameters == null)
                        VariabelByName(il, localCall.Identifier);
                    else
                    {
                        localCall.Parameters.ForEach(exp => GenerateExpression(il, exp));
                        var method = classes[_currentClass.SelfClassName.Identifier]
                            .MethodBuilders[localCall.Identifier];
                        il.EmitCall(OpCodes.Call, method, new Type[0]);
                    }
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

        private void VariabelByName(ILGenerator il, string identifier, bool isSet=false)
        {
            var isParam = false;
            var parCount = 0;
            var varCount = 0;
            foreach (var pair in _currentMethod.VariableDeclarations)
            {
                switch (pair.Value)
                {
                    case VariableDeclaration variableDeclaration:
                        if (variableDeclaration.Identifier == identifier)
                            goto exit;
                        varCount++;
                        break;
                    case ParameterDeclaration parameterDeclaration:
                        if (parameterDeclaration.Identifier == identifier)
                        {
                            isParam = true;
                            goto exit;
                        }
                        parCount++;
                        break;
                }
            }
            exit:
            if (isParam)
                il.Emit(isSet ? OpCodes.Starg : OpCodes.Ldarg, parCount);
            else
                il.Emit(isSet ? OpCodes.Stloc : OpCodes.Ldloc, varCount);
        }

        private void PrintAllVariables(ILGenerator il)
        {
            var locals = classes[_currentClass.SelfClassName.Identifier].Locals.ContainsKey(_currentMethod.Identifier)
                ? classes[_currentClass.SelfClassName.Identifier].Locals[_currentMethod.Identifier]
                : new List<LocalBuilder>();
            foreach (var local in locals)
            {
                if (local.LocalType == typeof(int) || local.LocalType == typeof(bool))
                    il.EmitWriteLine(local);
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
                    type = classes[t].TypeBuilder;
                    break;
            }
            var local = il.DeclareLocal(type);
            List<LocalBuilder> locals;
            if (classes[_currentClass.SelfClassName.Identifier].Locals.ContainsKey(_currentMethod.Identifier))
                locals = classes[_currentClass.SelfClassName.Identifier].Locals[_currentMethod.Identifier];
            else
            {
                locals = new List<LocalBuilder>();
                classes[_currentClass.SelfClassName.Identifier].Locals.Add(_currentMethod.Identifier, locals);
            }
            locals.Add(local);
            il.Emit(t == null ? OpCodes.Ldc_I4_0 : OpCodes.Ldnull);
            il.Emit(OpCodes.Stloc, local);
        }
    }
}