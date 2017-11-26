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
            
            var and = new MethodDeclaration("And")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            and.Parameters.Add(new ParameterDeclaration("b", new ClassName("Boolean")));
            var or = new MethodDeclaration("Or")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            or.Parameters.Add(new ParameterDeclaration("b", new ClassName("Boolean")));
            var not = new MethodDeclaration("Not")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            not.Parameters.Add(new ParameterDeclaration("b", new ClassName("Boolean")));
            var xor = new MethodDeclaration("Xor")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            xor.Parameters.Add(new ParameterDeclaration("b", new ClassName("Boolean")));

            var toInt = new MethodDeclaration("ToInteger")
            {
                Parent = @class,
                ResultType = new ClassName("Integer")
            };
      

            
            @class.MemberDeclarations.Add(equals);
            @class.Members.Add("Equals", equals);
            @class.MemberDeclarations.Add(and);
            @class.Members.Add("And", and);
            @class.MemberDeclarations.Add(not);
            @class.Members.Add("Not", not);
            @class.MemberDeclarations.Add(or);
            @class.Members.Add("Or", or);
            @class.MemberDeclarations.Add(xor);
            @class.Members.Add("Xor", xor);
            @class.MemberDeclarations.Add(toInt);
            @class.Members.Add("ToInteger", toInt);

            return @class;
        }

        public static Class GenerateInteger()
        {
            var className = new ClassName("Integer");
            var @class = new Class(className);
            
            
            var toReal = new MethodDeclaration("ToReal")
            {
                Parent = @class,
                ResultType = new ClassName("Real")
            };
            
            
            var unMinus = new MethodDeclaration("UnaryMinus")
            {
                Parent = @class,
                ResultType = new ClassName("Integer")
            };

            var minus = new MethodDeclaration("Minus")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            minus.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var minusReal = new MethodDeclaration("Minus")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Real")
//            };
//            minusReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
           
            var plus = new MethodDeclaration("Plus")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            plus.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var plusReal = new MethodDeclaration("Plus")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Real")
//            };
//            plusReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
            var mult = new MethodDeclaration("Mult")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            mult.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var multReal = new MethodDeclaration("Mult")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Real")
//            };
//            multReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
            var div = new MethodDeclaration("Div")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            div.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var divReal = new MethodDeclaration("Div")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Real")
//            };
//            divReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
            var rem = new MethodDeclaration("Rem")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            rem.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
           
 
            
            var equals = new MethodDeclaration("Equals")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            equals.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var equalsReal = new MethodDeclaration("Equals")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            equalsReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
            var greater = new MethodDeclaration("Greater")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            greater.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var greaterReal = new MethodDeclaration("Greater")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            greaterReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
            var less = new MethodDeclaration("Less")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            less.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var lessReal = new MethodDeclaration("Less")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            lessReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
            var lessEqual = new MethodDeclaration("LessEqual")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            lessEqual.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var lessEqualReal = new MethodDeclaration("LessEqual")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            lessEqualReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
            var greaterEqual = new MethodDeclaration("GreaterEqual")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            greaterEqual.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
//            var greaterEqualReal = new MethodDeclaration("GreaterEqual")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            greaterEqualReal.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));


            @class.MemberDeclarations.Add(toReal);
            @class.Members.Add("ToReal", toReal);
            @class.MemberDeclarations.Add(unMinus);
            @class.Members.Add("UnaryMinus", unMinus);
            @class.MemberDeclarations.Add(minus);
            @class.Members.Add("Minus", minus);
//            @class.MemberDeclarations.Add(minusReal);
//            @class.Members.Add("Minus", minusReal);
            @class.MemberDeclarations.Add(plus);
            @class.Members.Add("Plus", plus);
//            @class.MemberDeclarations.Add(plusReal);
//            @class.Members.Add("Plus", plusReal);
            @class.MemberDeclarations.Add(mult);
            @class.Members.Add("Mult", mult);
//            @class.MemberDeclarations.Add(multReal);
//            @class.Members.Add("Mult", multReal);
            @class.MemberDeclarations.Add(div);
            @class.Members.Add("Div", div);
//            @class.MemberDeclarations.Add(divReal);
//            @class.Members.Add("Div", divReal);
            @class.MemberDeclarations.Add(rem);
            @class.Members.Add("Rem", rem);
            
            
            @class.MemberDeclarations.Add(equals); 
            @class.Members.Add("Equals", equals);
//            @class.MemberDeclarations.Add(equalsReal); 
//            @class.Members.Add("Equals", equalsReal);
            @class.MemberDeclarations.Add(greater);
            @class.Members.Add("Greater", greater);
//            @class.MemberDeclarations.Add(greaterReal);
//            @class.Members.Add("Greater", greaterReal);
            @class.MemberDeclarations.Add(less);
            @class.Members.Add("Less", less);
//            @class.MemberDeclarations.Add(lessReal);
//            @class.Members.Add("Less", lessReal);
            @class.MemberDeclarations.Add(lessEqual);
            @class.Members.Add("LessEqual", lessEqual);
//            @class.MemberDeclarations.Add(lessEqualReal);
//            @class.Members.Add("LessEqual", lessEqualReal);
            @class.MemberDeclarations.Add(greaterEqual);
            @class.Members.Add("GreaterEqual", greaterEqual);
//            @class.MemberDeclarations.Add(greaterEqualReal);
//            @class.Members.Add("GreaterEqual", greaterEqualReal);
            return @class;
        }
        
        public static Class GenerateReal()
        {
            var className = new ClassName("Real");
            var @class = new Class(className);

            var unMinus = new MethodDeclaration("UnaryMinus")
            {
                Parent = @class,
                ResultType = new ClassName("Real")
            };
            
            var minus = new MethodDeclaration("Minus")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            minus.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var minusInt = new MethodDeclaration("Minus")
//            {
//                Parent = @class,
//                ResultType = new ClassName(className)
//            };
//            minusInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            
            var plus = new MethodDeclaration("Plus")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            plus.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var plusInt = new MethodDeclaration("Plus")
//            {
//                Parent = @class,
//                ResultType = new ClassName(className)
//            };
//            plusInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            var mult = new MethodDeclaration("Mult")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            mult.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var multInt = new MethodDeclaration("Mult")
//            {
//                Parent = @class,
//                ResultType = new ClassName(className)
//            };
//            multInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            var div = new MethodDeclaration("Div")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            div.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var divInt = new MethodDeclaration("Div")
//            {
//                Parent = @class,
//                ResultType = new ClassName(className)
//            };
//            divInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            var rem = new MethodDeclaration("Rem")
            {
                Parent = @class,
                ResultType = new ClassName(className)
            };
            rem.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
    
            
            var equals = new MethodDeclaration("Equals")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            equals.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var equalsInt = new MethodDeclaration("Equals")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            equalsInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            var greater = new MethodDeclaration("Greater")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            greater.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var greaterInt = new MethodDeclaration("Greater")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            greaterInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            var less = new MethodDeclaration("Less")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            less.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var lessInt = new MethodDeclaration("Less")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            lessInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            var lessEqual = new MethodDeclaration("LessEqual")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            lessEqual.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var lessEqualInt = new MethodDeclaration("LessEqual")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            lessEqualInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            
            var greaterEqual = new MethodDeclaration("GreaterEqual")
            {
                Parent = @class,
                ResultType = new ClassName("Boolean")
            };
            greaterEqual.Parameters.Add(new ParameterDeclaration("b", new ClassName("Real")));
            
//            var greaterEqualInt = new MethodDeclaration("GreaterEqual")
//            {
//                Parent = @class,
//                ResultType = new ClassName("Boolean")
//            };
//            greaterEqualInt.Parameters.Add(new ParameterDeclaration("b", new ClassName("Integer")));
            var toInt = new MethodDeclaration("ToInteger")
            {
                Parent = @class,
                ResultType = new ClassName("Integer")
            };
            
            

            @class.MemberDeclarations.Add(toInt);
            @class.Members.Add("ToInteger", toInt);
            @class.MemberDeclarations.Add(unMinus);
            @class.Members.Add("UnaryMinus", unMinus);
            @class.MemberDeclarations.Add(minus);
            @class.Members.Add("Minus", minus);
//            @class.MemberDeclarations.Add(minusReal);
//            @class.Members.Add("Minus", minusReal);
            @class.MemberDeclarations.Add(plus);
            @class.Members.Add("Plus", plus);
//            @class.MemberDeclarations.Add(plusInt);
//            @class.Members.Add("Plus", plusInt);
            @class.MemberDeclarations.Add(mult);
            @class.Members.Add("Mult", mult);
//            @class.MemberDeclarations.Add(multInt);
//            @class.Members.Add("Mult", multInt);
            @class.MemberDeclarations.Add(div);
            @class.Members.Add("Div", div);
//            @class.MemberDeclarations.Add(divInt);
//            @class.Members.Add("Div", divInt);
            @class.MemberDeclarations.Add(rem);
            @class.Members.Add("Rem", rem);
            
            
            @class.MemberDeclarations.Add(equals); 
            @class.Members.Add("Equals", equals);
//            @class.MemberDeclarations.Add(equalsInt); 
//            @class.Members.Add("Equals", equalsInt);
            @class.MemberDeclarations.Add(greater);
            @class.Members.Add("Greater", greater);
//            @class.MemberDeclarations.Add(greaterInt);
//            @class.Members.Add("Greater", greaterInt);
            @class.MemberDeclarations.Add(less);
            @class.Members.Add("Less", less);
//            @class.MemberDeclarations.Add(lessInt);
//            @class.Members.Add("Less", lessInt);
            @class.MemberDeclarations.Add(lessEqual);
            @class.Members.Add("LessEqual", lessEqual);
//            @class.MemberDeclarations.Add(lessEqualInt);
//            @class.Members.Add("LessEqual", lessEqualInt);
            @class.MemberDeclarations.Add(greaterEqual);
            @class.Members.Add("GreaterEqual", greaterEqual);
//            @class.MemberDeclarations.Add(greaterEqualInt);
//            @class.Members.Add("GreaterEqual", greaterEqualInt);
            return @class;
        }

//        public static Class GenerateArray()
//        {
//            var className = new ClassName("Array");
//            var @class = new Class(className);
//            
//            var length = new MethodDeclaration("Length");
//            {
//                Parent = @class,
//                ResultType = new ClassName("Array");
//            }
//            var get = new MethodDeclaration("Get");
//            {
//                Parent = @class,
//                ResultType = new ClassName("Array");
//            }
//            
//            var set = new MethodDeclaration("Set");
//            {
//                Parent = @class,
//                ResultType = new ClassName("array");
//            }
//            
//            
//
//            @class.MemberDeclarations.Add(length);
//            @class.Members.Add("Length", length);
//            @class.MemberDeclarations.Add(get);
//            @class.Members.Add("Get", get);
//            @class.MemberDeclarations.Add(set);
//            @class.Members.Add("Set", set);
//            return @class;
//        }
    }
}