using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

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

        private Dictionary<string, ClassStructure> classes = new Dictionary<string, ClassStructure>();

        public void GenerateProgram()
        {
            AssemblyName an = new AssemblyName {Name = Path.GetFileNameWithoutExtension("test generator")};
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder modb = ab.DefineDynamicModule(an.Name, an.Name + ".exe", true);

            modb.CreateGlobalFunctions();

            TypeAttributes classAttrs = TypeAttributes.Public;
            TypeBuilder typeBuilder = modb.DefineType("ClassName", classAttrs);
            classes["ClassName"] = new ClassStructure
            {
                typeBuilder = typeBuilder,
                methodBuilders = new Dictionary<string, MethodBuilder>(),
                fieldBuilders = new Dictionary<string, FieldBuilder>()
            };

            Type[] ctorTypes = new Type[0];
            ConstructorBuilder ctorBuilder =
                typeBuilder.DefineConstructor(
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.Standard,
                    ctorTypes);


            ILGenerator ctorIL = ctorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);

            Type[] ctorArgs = new Type[0];
            ConstructorInfo ctor = typeof(object).GetConstructor(ctorArgs);
            ctorIL.Emit(OpCodes.Call, ctor);
            ctorIL.Emit(OpCodes.Ret);

            typeBuilder = classes["ClassName"].typeBuilder;
            var @class = classes["ClassName"];

            MethodAttributes methodAttrs = MethodAttributes.Public | MethodAttributes.Static;
            Type resType = typeof(void);
            Type[] parTypes = new Type[0];
            MethodBuilder mb = typeBuilder.DefineMethod("Main", methodAttrs, resType, parTypes);
            @class.methodBuilders.Add("Main", mb);


            MethodBuilder methodBuilder = @class.methodBuilders["Main"];

            ILGenerator il = methodBuilder.GetILGenerator();
            il.EmitWriteLine("123");
            il.Emit(OpCodes.Ret);

            ab.SetEntryPoint(methodBuilder);

            typeBuilder.CreateType();
            // Saving the assembly
            ab.Save(Path.GetFileName("test generator.exe"));
        }
    }
}