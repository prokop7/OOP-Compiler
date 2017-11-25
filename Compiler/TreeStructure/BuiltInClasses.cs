using Compiler.TreeStructure.MemberDeclarations;

namespace Compiler.TreeStructure
{
    public static class BuiltInClasses
    {
        public static Class GenerateBoolean()
        {
            var className = new ClassName("Boolean");
            var @class = new Class(className);

            var equals = new MethodDeclaration("Equals") {Parent = @class};
            equals.ResultType = new ClassName(className);

            equals.Parameters.Add(new ParameterDeclaration("b", new ClassName("Boolean")));
            @class.MemberDeclarations.Add(equals);
            @class.Members.Add("Equals", equals);

            return @class;
        }

        public static Class GenerateInteger()
        {
            var className = new ClassName("Integer");
            var @class = new Class(className);

            var minus = new MethodDeclaration("Minus")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            minus.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));


            var equals = new MethodDeclaration("Equals")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            equals.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            var greater = new MethodDeclaration("Greater")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            greater.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));


            @class.MemberDeclarations.Add(minus);
            @class.Members.Add("Minus", minus);
            @class.MemberDeclarations.Add(equals);
            @class.Members.Add("Equals", equals);
            @class.MemberDeclarations.Add(greater);
            @class.Members.Add("Greater", greater);
            return @class;
        }
    }
}