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

        private string Filename { get; }
        private readonly Dictionary<string, ClassStructure> classes = new Dictionary<string, ClassStructure>();
        private readonly List<Class> _classList;
        private Class _currentClass;
        private MethodDeclaration _currentMethod;
        private ConstructorDeclaration _currentCtor;

        public Generator(List<Class> classList, string filename)
        {
            _classList = classList;
            Filename = filename;
        }

        public void GenerateProgram()
        {
            Log("Code generating: start", 1);
            var an = new AssemblyName {Name = Path.GetFileNameWithoutExtension(Filename)};
            var ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            var modb = ab.DefineDynamicModule(an.Name, an.Name + ".exe", true);

            modb.CreateGlobalFunctions();

            // Fill class info
            foreach (var cls in _classList)
            {
                Log($"Creating class {cls}", 3);
                var classAttrs = TypeAttributes.Public;
                var typeBuilder = modb.DefineType(cls.SelfClassName.Identifier,
                    classAttrs,
                    GetTypeByClassIdentifier(cls.BaseClassName?.Identifier));
                classes[cls.SelfClassName.Identifier] = new ClassStructure
                {
                    TypeBuilder = typeBuilder,
                    MethodBuilders = new Dictionary<string, MethodBuilder>(),
                    FieldBuilders = new Dictionary<string, FieldBuilder>(),
                    Locals = new Dictionary<string, List<LocalBuilder>>()
                };
            }

            // Define custom ctors and fields.
            foreach (var cls in _classList)
            {
                var typeBuilder = classes[cls.SelfClassName.Identifier].TypeBuilder;
                _currentClass = cls;
                foreach (var memberDeclaration in cls.MemberDeclarations)
                    switch (memberDeclaration)
                    {
                        case ConstructorDeclaration constructorDeclaration:
                            Log($"Creating constructor {constructorDeclaration}", 4);
                            _currentMethod = null;
                            _currentCtor = constructorDeclaration;
                            var ctorTypes = new Type[constructorDeclaration.Parameters.Count];
                            for (var j = 0; j < constructorDeclaration.Parameters.Count; j++)
                            {
                                var t = GetTypeByClassIdentifier(constructorDeclaration.Parameters[j].Type
                                    .Identifier);
                                ctorTypes[j] = t;
                            }
                            var ctorBuilder = typeBuilder.DefineConstructor(
                                MethodAttributes.Public | MethodAttributes.SpecialName |
                                MethodAttributes.RTSpecialName,
                                CallingConventions.Standard,
                                ctorTypes);

                            classes[cls.SelfClassName.Identifier].CtorBuilder = ctorBuilder;

                            var ctorIl = ctorBuilder.GetILGenerator();
//                            ctorIl.Emit(OpCodes.Ldarg_0);
                            // Generating constructor locals
                            foreach (var pair in constructorDeclaration.VariableDeclarations)
                                if (pair.Value is VariableDeclaration value)
                                    GenerateLocal(ctorIl, value);
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

            // If class hasn't ctor create it.
            foreach (var cls in _classList)
            {
                var typeBuilder = classes[cls.SelfClassName.Identifier].TypeBuilder;
                _currentClass = cls;
                if (!cls.MemberDeclarations.Any(m => m is ConstructorDeclaration))
                {
                    var ctorTypes = new Type[0];
                    var ctorBuilder = typeBuilder.DefineConstructor(
                        MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                        CallingConventions.Standard,
                        ctorTypes);
                    classes[cls.SelfClassName.Identifier].CtorBuilder = ctorBuilder;

                    var ctorIl = ctorBuilder.GetILGenerator();

                    foreach (var el in cls.MemberDeclarations.Where(m => m is VariableDeclaration).ToList())
                    {
                        ctorIl.Emit(OpCodes.Ldarg_0);
                        var varDeclaration = (VariableDeclaration) el;
                        var fb = classes[cls.SelfClassName.Identifier].FieldBuilders[varDeclaration.Identifier];
                        GenerateExpression(ctorIl, varDeclaration.Expression);
                        ctorIl.Emit(OpCodes.Stfld, fb);
                    }

                    ctorIl.Emit(OpCodes.Ldarg_0);
                    var ctorArgs = new Type[0];
                    //TODO add base instead of object
                    var ctor = GetTypeByClassIdentifier(cls.BaseClassName?.Identifier).GetConstructor(ctorArgs);
//                    var ctor = typeof(object).GetConstructor(ctorArgs);
                    ctorIl.Emit(OpCodes.Call, ctor ?? throw new NullReferenceException());
                    ctorIl.Emit(OpCodes.Ret);
                }
            }

            // Defining methods
            foreach (var cls in _classList)
            {
                var typeBuilder = classes[cls.SelfClassName.Identifier].TypeBuilder;
                _currentClass = cls;
                foreach (var memberDeclaration in cls.MemberDeclarations)
                {
                    switch (memberDeclaration)
                    {
                        case MethodDeclaration method:
                            Log($"Creating method {method}", 4);
                            var methodAttrs = MethodAttributes.Public;
                            if (method.Identifier == "Main") methodAttrs |= MethodAttributes.Static;
                            else
                                methodAttrs |= MethodAttributes.Virtual;

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
                    }
                }
                // Generating class bodies
            }

            // Creating ctor bodies and init fields
            // Filling methods
            foreach (var cls in _classList)
            {
                var typeBuilder = classes[cls.SelfClassName.Identifier].TypeBuilder;
                _currentClass = cls;
                foreach (var memberDeclaration in cls.MemberDeclarations)
                {
                    switch (memberDeclaration)
                    {
                        case ConstructorDeclaration constructorDeclaration:

                            _currentMethod = null;
                            _currentCtor = constructorDeclaration;
                            var ctorBuilder = classes[cls.SelfClassName.Identifier].CtorBuilder;
                            var ctorIl = ctorBuilder.GetILGenerator();

                            foreach (var el in cls.MemberDeclarations.Where(m => m is VariableDeclaration).ToList())
                            {
                                ctorIl.Emit(OpCodes.Ldarg_0);
                                var varDeclaration = (VariableDeclaration) el;
                                var fb = classes[cls.SelfClassName.Identifier].FieldBuilders[varDeclaration.Identifier];
                                if (varDeclaration.IsDeclared)
                                    GenerateExpression(ctorIl, varDeclaration.Expression);
                                else
                                    GenerateDefaultExpression(ctorIl, varDeclaration.Expression);
                                ctorIl.Emit(OpCodes.Stfld, fb);
                            }

                            ctorIl.Emit(OpCodes.Ldarg_0);
                            var ctorArgs = new Type[0];
//                            var ctor = typeof(object).GetConstructor(ctorArgs);
                            var ctor = GetTypeByClassIdentifier(cls.BaseClassName?.Identifier).GetConstructor(ctorArgs);
                            ctorIl.Emit(OpCodes.Call, ctor ?? throw new NullReferenceException());
//                            ctorIl.Emit(OpCodes.Ldarg_0);

                            foreach (var body in constructorDeclaration.Body)
                                GenerateStatement(ctorIl, body);
                            PrintAllVariables(ctorIl);
                            ctorIl.Emit(OpCodes.Ret);
                            break;
                        case MethodDeclaration method:
                            Log($"Filling method {method}", 5);
                            _currentMethod = method;
                            _currentCtor = null;
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
                            if (!(last is ReturnStatement) && method.ResultType == null)
                            {
                                PrintAllVariables(il);
                                il.Emit(OpCodes.Ret);
                            }

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
            ab.Save(Path.GetFileName(an.Name + ".exe"));
            Log("Code generating: finish", 1);

            Log($"Output file = {Path.GetFullPath(an.Name + ".exe")}", 0);
        }

        private void GenerateDefaultExpression(ILGenerator il, Expression expression)
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
                            il.Emit(OpCodes.Ldnull);
                            break;
                    }
                    break;
            }
        }

        private Type GetTypeByClassIdentifier(string identifier)
        {
            switch (identifier)
            {
                case "Integer":
                    return typeof(int);
                case "Boolean":
                    return typeof(bool);
                case "Real":
                    return typeof(double);
                case null:
                    return typeof(object);
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
            var i = _currentMethod?.VariableDeclarations
                        .Where(pair => pair.Value is VariableDeclaration)
                        .TakeWhile(pair => pair.Key != variableDeclaration.Identifier)
                        .Count() ??
                    _currentCtor.VariableDeclarations
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

            IBody lastBody = null;
            foreach (var e in ifStmt.Body)
            {
                GenerateStatement(il, e);
                lastBody = e;
            }

            if (ifStmt.ElseBody != null)
            {
                var branchExit = il.DefineLabel();

                if (!(lastBody is ReturnStatement))
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
            var cls = _currentClass;
            var declaration = VariableDeclarationChecker.GetTypeVariable(assignment, assignment.Identifier);
            if (declaration == null)
            {
                check:
                if (cls.Base != null && !cls.Members.ContainsKey(assignment.Identifier))
                {
                    cls = cls.Base;
                    goto check;
                }
                declaration = cls.Members[assignment.Identifier];
            }
            if (declaration.Parent is Class)
            {
                // Set values for field
                if (declaration is VariableDeclaration vDecl)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    var fb = classes[cls.SelfClassName.Identifier].FieldBuilders[vDecl.Identifier];
                    GenerateExpression(il, assignment.Expression);
                    il.Emit(OpCodes.Stfld, fb);
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
                                var cls = StaticTables.ClassTable[expressionCall.InputType][0];
                                check:
                                if (!cls.Members.ContainsKey(expressionCall.Identifier))
                                {
                                    cls = cls.Base;
                                    goto check;
                                }
                                var method = classes[cls.SelfClassName.Identifier]
                                    .MethodBuilders[expressionCall.Identifier];
                                var paramsTypes = new Type[call.Arguments.Count];
                                for (var i = 0; i < call.Arguments.Count; i++)
                                {
                                    var arg = call.Arguments[i];
                                    paramsTypes[i] = GetTypeByClassIdentifier(arg.ReturnType);
                                    GenerateExpression(il, arg);
                                }
                                il.EmitCall(OpCodes.Callvirt, method, paramsTypes);
//                                if (method.ReturnType != typeof(void))
//                                    il.Emit(OpCodes.Pop);
                                break;
                        }
                        break;
                    case FieldCall fieldCall:
                        //TODO choose class
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
                case "Equal":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Ceq);
                    break;
                case "And":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.And);
                    break;
                case "Or":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Or);
                    break;
                case "Not":
                    il.Emit(OpCodes.Not);
                    break;
                case "Xor":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Xor);
                    break;
                case "ToInteger":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Conv_I4);
                    break;
                default:
                    throw new InvalidOperationException();
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
                case "Equal":
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
                case "ToReal":
                    il.Emit(OpCodes.Conv_R8);
                    break;
                case "ToBoolean":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Conv_I4);
                    break;
                case "UnaryMinus":
                    il.Emit(OpCodes.Neg);
                    break;
                default:
                    throw new InvalidOperationException();
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
                case "Equal":
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
                case "ToInteger":
                    GenerateExpression(il, call.Arguments[0]);
                    il.Emit(OpCodes.Conv_I4);
                    break;
                case "UnaryMinus":
                    il.Emit(OpCodes.Neg);
                    break;
                default:
                    throw new InvalidOperationException();
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
                    if (localCall.Arguments == null)
                    {
                        var @class = _currentClass;
                        var pClass = @class;
                        rec:
                        if (pClass.Base != null && !pClass.Members.ContainsKey(localCall.Identifier))
                        {
                            pClass = pClass.Base;
                            goto rec;
                        }
                        if (pClass.Members.ContainsKey(localCall.Identifier))
                            @class = pClass;
                        if (@class.Members.ContainsKey(localCall.Identifier))
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            //TODO choose class
//                            var cls = StaticTables.ClassTable[@class.SelfClassName.Identifier][0];
//                            check:
//                            if (!cls.Members.ContainsKey(localCall.Identifier))
//                            {
//                                cls = cls.Base;
//                                goto check;
//                            }
                            var fb = classes[@class.SelfClassName.Identifier].FieldBuilders[localCall.Identifier];
                            il.Emit(OpCodes.Ldfld, fb);
                        }
                        else
                            VariabelByName(il, localCall.Identifier);
                    }
                    else
                    {
                        var @class = _currentClass;
                        var pClass = @class;
                        rec:
                        if (pClass.Base != null && !pClass.Members.ContainsKey(localCall.Identifier))
                        {
                            pClass = pClass.Base;
                            goto rec;
                        }
                        if (pClass.Members.ContainsKey(localCall.Identifier))
                            @class = pClass;

                        il.Emit(OpCodes.Ldarg_0);
                        localCall.Arguments.ForEach(exp => GenerateExpression(il, exp));
                        var method = classes[@class.SelfClassName.Identifier]
                            .MethodBuilders[localCall.Identifier];
                        il.EmitCall(OpCodes.Call, method, new Type[0]);
                    }
                    break;
                case BooleanLiteral booleanLiteral:
                    il.Emit(OpCodes.Ldc_I4, booleanLiteral.Value ? 1 : 0);
                    break;
                case ConstructorCall constructorCall:
                    var id = constructorCall.ClassName.Identifier;
                    foreach (var arg in constructorCall.Arguments)
                        GenerateExpression(il, arg);
                    switch (id)
                    {
                        case "Integer":
                        case "Boolean":
                        case "Real":
                            break;
                        default:
                            var ctor = classes[id].CtorBuilder;
                            il.Emit(OpCodes.Newobj, ctor);
                            break;
                    }
                    break;
                case IntegerLiteral integerLiteral:
                    il.Emit(OpCodes.Ldc_I4, integerLiteral.Value);
                    break;
                case RealLiteral realLiteral:
                    il.Emit(OpCodes.Ldc_R8, realLiteral.Value);
                    break;
            }
        }

        private void VariabelByName(ILGenerator il, string identifier, bool isSet = false)
        {
            var isParam = false;
            var parCount = 1;
            var varCount = 0;
            if (_currentMethod != null)
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
            else
            {
                foreach (var pair in _currentCtor.VariableDeclarations)
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
            }
            exit:
            if (isParam)
                il.Emit(isSet ? OpCodes.Starg : OpCodes.Ldarg, parCount);
            else
                il.Emit(isSet ? OpCodes.Stloc : OpCodes.Ldloc, varCount);
        }

        private void PrintAllVariables(ILGenerator il)
        {
            var locals = (_currentMethod != null)
                    ? classes[_currentClass.SelfClassName.Identifier].Locals.ContainsKey(_currentMethod.Identifier)
                        ? classes[_currentClass.SelfClassName.Identifier].Locals[_currentMethod.Identifier]
                        : new List<LocalBuilder>()
                    : classes[_currentClass.SelfClassName.Identifier].Locals.ContainsKey(_currentCtor.ToString())
                        ? classes[_currentClass.SelfClassName.Identifier].Locals[_currentCtor.ToString()]
                        : new List<LocalBuilder>()
                ;
            foreach (var local in locals)
            {
                if (local.LocalType == typeof(int) || local.LocalType == typeof(bool) ||
                    local.LocalType == typeof(double))
                    il.EmitWriteLine(local);
            }
        }

        private void GenerateLocal(ILGenerator il, VariableDeclaration variableDeclaration)
        {
            var t = variableDeclaration.Classname == null
                ? variableDeclaration.Expression.ReturnType
                : variableDeclaration.Classname.Identifier;
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
            var name = _currentMethod == null ? _currentCtor.ToString() : _currentMethod.Identifier;

            if (classes[_currentClass.SelfClassName.Identifier].Locals.ContainsKey(name))
                locals = classes[_currentClass.SelfClassName.Identifier].Locals[name];
            else
            {
                locals = new List<LocalBuilder>();
                classes[_currentClass.SelfClassName.Identifier].Locals.Add(name, locals);
            }
            locals.Add(local);
//            il.Emit(t == null ? OpCodes.Ldc_I4_0 : OpCodes.Ldnull);
//            il.Emit(OpCodes.Stloc, local);
        }
    }
}