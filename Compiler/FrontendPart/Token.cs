using System;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;

namespace Compiler.FrontendPart
{
    public enum Type { 
        [Description("\'false\'")] False,
        [Description("\'true\'")] True,
        [Description("\':\'")] Colon,
        [Description("\',\'")] Comma,
        [Description("\'.\'")] Dot,
        [Description("\'(\'")] Lparen,
        [Description("\')\'")] Rparen,
        [Description("\'[\'")] SqrtLparen,
        [Description("\']\'")] SqrtRparen,
        [Description("\'<\'")] TrglLparen,
        [Description("\'>\'")] TrglRparen,
        [Description("\':=\'")] Assignment,
        [Description("\'is\'")] IsKey,
        [Description("\'if\'")] IfKey,
        [Description("\'var\'")] VarKey,
        [Description("\'end\'")] EndKey,
        [Description("\'else\'")] ElseKey,
        [Description("\'extends\'")] ExtendsKey,
        [Description("\'this\'")] ThisKey,
        [Description("\'then\'")] ThenKey,
        [Description("\'loop\'")] LoopKey,
        [Description("\'base\'")] BaseKey,
        [Description("\'class\'")] ClassKey,
        [Description("\'while\'")] WhileKey,
        [Description("\'method\'")] MethodKey,
        [Description("\'return\'")] ReturnKey,
        [Description("Identifier")] Id, 
        [Description("Numeric literal")] Num
    } 
    public class Token
    {
        public Type type { get; set; }
        public Object value { get; set; }
        public int lineNumber { get; }
        public int positionNumber { get; }

        public Token(Type type, int lineNumber, int positionNumber, object value = null)
        {
            this.type = type;
            this.lineNumber = lineNumber;
            this.positionNumber = positionNumber;
            this.value = value;
        }

        public static string StringValueOf(Type value) 
        { 
            var fi = value.GetType().GetField(value.ToString()); 
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false); 
            return attributes.Length > 0 ? 
                attributes[0].Description
                : 
                value.ToString(); 

        }

        public override string ToString()
        {
            return ((value != null) ? ($"{value}") : $"{StringValueOf(type)}");
        }
    }
}