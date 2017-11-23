using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Compiler.FrontendPart.SemanticAnalyzer;
using Compiler.TreeStructure;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

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

                var ctorIL = ctorBuilder.GetILGenerator();
                ctorIL.Emit(OpCodes.Ldarg_0);

                var ctorArgs = new Type[0];
                var ctor = typeof(System.Object).GetConstructor(ctorArgs);
                ctorIL.Emit(OpCodes.Call, ctor);
                ctorIL.Emit(OpCodes.Ret);

                foreach (var memberDeclaration in cls.MemberDeclarations)
                {
                    switch (memberDeclaration)
                    {
                        case ConstructorDeclaration constructorDeclaration:
                            break;
                        case MethodDeclaration method:
                            var methodAttrs = MethodAttributes.Public;
                            if (method.Identifier == "Main") methodAttrs |= MethodAttributes.Static;


                            Type resType = null;
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
                                Type parType = null;
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
                TypeBuilder typeBuilder = classes[cls.SelfClassName.Identifier].typeBuilder;
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
                            MethodBuilder methodBuilder =
                                classes[cls.SelfClassName.Identifier].methodBuilders[method.Identifier];

                            ILGenerator il = methodBuilder.GetILGenerator();

                            // Generating method locals
                            foreach (var parameter in method.Parameters)
                                GenerateLocal(il, parameter);

                            // Generating method's bodies
                            IBody last = null;
                            foreach (var body in method.Body)
                            {
                                generateStatement(il, body);
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

        private void generateStatement(ILGenerator il, IBody body)
        {
            throw new NotImplementedException();
        }

        private void GenerateLocal(ILGenerator il, ParameterDeclaration field)
        {
            ClassName t = field.Type;
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
            LocalBuilder local = il.DeclareLocal(type);
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