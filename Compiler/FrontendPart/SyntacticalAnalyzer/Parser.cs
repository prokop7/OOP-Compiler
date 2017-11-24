using System;
using System.Collections.Generic;
using Compiler.Exceptions;
using Compiler.FrontendPart.LexicalAnalyzer;
using Compiler.TreeStructure;
using Compiler.TreeStructure.Expressions;
using Compiler.TreeStructure.MemberDeclarations;
using Compiler.TreeStructure.Statements;

namespace Compiler.FrontendPart.SyntacticalAnalyzer
{
    public class Parser
    {
        private Lexer lexer;
        private Token currentToken = null;
        private Token prevToken = null;
        

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }

        private Token GetNextToken()
        {
            prevToken = currentToken;
            currentToken = lexer.GetNextToken();
            Console.WriteLine(currentToken);
            return currentToken;
        }

        private Token PeekCurrentToken()
        {
            return currentToken;
        }

        public List<Class> Analyze()
        {
            var tree = new List<Class>();
            Class classNode = null;
            while ((classNode = ParseClass()) != null){
                    tree.Add(classNode);
            }
            
            return tree;
        }

        /* Checks obligatory order of tokens */
        private void CheckTokenTypeStrong(Token token, Type expectedType, string expectation = " token")
        {
            if (token == null)
                throw new UnexpectedTokenException(prevToken.lineNumber, prevToken.positionNumber, Token.StringValueOf(expectedType) + expectation);
            if(!token.type.Equals(expectedType))
                throw new UnexpectedTokenException(token.lineNumber, token.positionNumber, Token.StringValueOf(expectedType) + expectation);
        }
        
        /* Checks optional tokens */
        private bool CheckTokenTypeWeak(Token token, Type expectedType)
        {
            if (token == null)
                throw new UnexpectedTokenException(prevToken.lineNumber, prevToken.positionNumber);
            return token.type.Equals(expectedType);
        }
        
        private Class ParseClass()
        {
            if (GetNextToken() == null)
                return null;
            CheckTokenTypeStrong(PeekCurrentToken(), Type.ClassKey);
            GetNextToken();
            var classObj = new Class(ParseClassName());
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.ExtendsKey))
            {
                GetNextToken();
                var baseClassName = ParseClassName();
                if(baseClassName == null)
                    throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "base class name");
                classObj.BaseClassName.Parent = classObj;
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.IsKey);

            /* Parse body of class */
            IMemberDeclaration memberDeclaration;
            GetNextToken();
            while ((memberDeclaration = ParseMemberDeclaration()) != null)
            {
                memberDeclaration.Parent = classObj;
                classObj.MemberDeclarations.Add(memberDeclaration);
                GetNextToken();
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return classObj;
        }

        private ClassName ParseClassName()
        {
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Id, " of class name");
            var className = new ClassName((string)PeekCurrentToken().value);

            GetNextToken();
            var genericParams = ParseGenericParams();
            if (genericParams != null)
            {
                foreach (var genericParam in genericParams)
                {
                    genericParam.Parent = className;
                    className.Specification.Add(genericParam);
                }
            }
            return className;

        }

        private List<ClassName> ParseGenericParams()
        {
            if (!CheckTokenTypeWeak(PeekCurrentToken(), Type.SqrtLparen))
                return null;
            var genericParams = new List<ClassName>();
            do
            {
                GetNextToken();
                var param = ParseClassName();
                if (param == null)
                    throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().lineNumber, "class name");
                genericParams.Add(param);
            } while (CheckTokenTypeWeak(PeekCurrentToken(), Type.Comma));
            CheckTokenTypeStrong(PeekCurrentToken(), Type.SqrtRparen);
            GetNextToken();
            return genericParams;
        }

        private IMemberDeclaration ParseMemberDeclaration()
        {
            switch (PeekCurrentToken().type)
            {
                case Type.EndKey:
                    return null;
                case Type.VarKey:
                    return ParseVariableDeclaration();
                case Type.MethodKey:
                    return ParseMethodDeclaration();
                case Type.ThisKey:
                    return ParseConstructorDeclaration();
                default:
                    throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
            }
        }

        private VariableDeclaration ParseVariableDeclaration()
        {
            CheckTokenTypeStrong(GetNextToken(), Type.Id, " of variable name");
            var varName = (string)PeekCurrentToken().value;
            var varDeclaration = new VariableDeclaration(varName);
            ClassName className = null;
            if (CheckTokenTypeWeak(GetNextToken(), Type.Colon))
            {
                GetNextToken();
                className = ParseClassName();
                if (className == null)
                    throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "class name");
                className.Parent = varDeclaration;
                varDeclaration.Classname = className;
            }
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.IsKey))
            {
                GetNextToken();
                var expression = ParseExpression();
                if (expression == null)
                    throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber,
                        "expression");
                expression.Parent = varDeclaration;
                varDeclaration.Expression = expression;
            }
            return varDeclaration;
        }

        private MethodDeclaration ParseMethodDeclaration()
        {
            CheckTokenTypeStrong(GetNextToken(), Type.Id, " of method name");
            var method = new MethodDeclaration((string)PeekCurrentToken().value);
            var parameters = ParseParameters();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    parameter.Parent = method;
                    method.Parameters.Add(parameter);
                }
            }
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.Colon))
            {
                var resultType = ParseClassName();
                if(resultType == null)
                    throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "class name");
                resultType.Parent = method;
                method.ResultType = resultType;
            }
            GetNextToken();
            var body = ParseBody();
            foreach (var bodyItem in body)
            {
                bodyItem.Parent = method;
                method.Body.Add(bodyItem);
            }
            
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);

            return method;
        }

        private ConstructorDeclaration ParseConstructorDeclaration()
        {
            var parameters = ParseParameters();
            //if (parameters != null)
            CheckTokenTypeStrong(PeekCurrentToken(), Type.IsKey);
            GetNextToken();
            var body = ParseBody();
            
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return new ConstructorDeclaration(parameters, body);
        }

        private Expression ParseExpression()
        {
            var primary = ParsePrimary();
            if (primary == null)
            {
                return null;
            }
            List<MethodOrFieldCall> calls = new List<MethodOrFieldCall>();
            //GetNextToken();
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.Lparen))
            {
                var args = ParseArguments();
                CheckTokenTypeStrong(PeekCurrentToken(), Type.Rparen);
                calls.Add(new MethodOrFieldCall(primary.ToString(), args));
                primary = null;
                GetNextToken();
            }
            while (CheckTokenTypeWeak(PeekCurrentToken(), Type.Dot))
            {
                CheckTokenTypeStrong(GetNextToken(), Type.Id);
                var id = (string)PeekCurrentToken().value;
                GetNextToken();
                var arguments = ParseArguments();
                calls.Add(new MethodOrFieldCall(id, arguments));
                GetNextToken();
            }
            return new Expression(primary, calls);
        }

        private List<ParameterDeclaration> ParseParameters()
        {
            if (!CheckTokenTypeWeak(GetNextToken(), Type.Lparen)) // if method declaration without empty parantheses
                return null;
            List<ParameterDeclaration> parameters = new List<ParameterDeclaration>();
            var parameter = ParseParameterDeclaration();
            if (parameter != null) // there is at least one parameter in parantheses
            {
                parameters.Add(parameter);
                while (CheckTokenTypeWeak(GetNextToken(), Type.Comma))
                {
                    parameter = ParseParameterDeclaration();
                    if (parameter == null) // if no parameter after comma
                        throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
                }
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Rparen);
            return parameters;
        }

        private ParameterDeclaration ParseParameterDeclaration()
        {
            if (!CheckTokenTypeWeak(GetNextToken(), Type.Id))
                return null;
            var paramName = (string)PeekCurrentToken().value;
            CheckTokenTypeStrong(GetNextToken(), Type.Colon);
            var className = ParseClassName();
            if (className == null)
                throw  new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "class name");
            return new ParameterDeclaration(paramName, className);
        }

        private List<IBody> ParseBody()
        {
            List<IBody> body = new List<IBody>();
            while (!CheckTokenTypeWeak(PeekCurrentToken(), Type.EndKey))
            {
                switch (PeekCurrentToken().type)
                {
                    case Type.VarKey:
                        body.Add(ParseVariableDeclaration());
                        break;
                    case Type.Id:
                        var id = (string)PeekCurrentToken().value;
                        if (CheckTokenTypeWeak(GetNextToken(), Type.Assignment))
                        {
                            GetNextToken();
                            var expression = ParseExpression();
                            if (expression == null)
                                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "expression");
                            body.Add(new Assignment(id, expression));
                            break;
                        }
                        if (CheckTokenTypeWeak(PeekCurrentToken(), Type.Dot)) // method call on object
                        {
                            CheckTokenTypeStrong(GetNextToken(), Type.Id);
                            var method = (string)PeekCurrentToken().value;
                            GetNextToken();
                            var arguments = ParseArguments();
                            if (arguments == null)
                            {
                                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "method call");
                            }
                            //body.Add(new Expression(new MethodOrFieldCall(method, arguments)));
                            break;
                        }
                        if (CheckTokenTypeWeak(PeekCurrentToken(), Type.Lparen)) // current class' method call
                        {
                            var method = (string)PeekCurrentToken().value;
                            GetNextToken();
                            var arguments = ParseArguments();
                            if (arguments == null)
                                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "method call");
                            body.Add(new Expression(new Fie));
                            break;
                        }
                        throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
                    case Type.WhileKey:
                        body.Add(ParseWhileLoop());
                        GetNextToken();
                        break;
                    case Type.IfKey:
                        body.Add(ParseIfStatement());
                        GetNextToken();
                        break;
                    case Type.ReturnKey:
                        body.Add(ParseReturnStatement());
                        GetNextToken();
                        break;
                    default:
                        throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
                }
            }
            return body;
        }

        private WhileLoop ParseWhileLoop()
        {
            GetNextToken();
            var expression = ParseExpression();
            if (expression == null)
                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "expression");
            CheckTokenTypeStrong(PeekCurrentToken(), Type.LoopKey);
            GetNextToken();
            var body = ParseBody();
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return new WhileLoop(expression, body);
        }

        private IfStatement ParseIfStatement()
        {
            GetNextToken();
            var expression = ParseExpression();
            if (expression == null)
                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "expression");
            CheckTokenTypeStrong(PeekCurrentToken(), Type.ThenKey);
            GetNextToken();
            var body = ParseBody();
            var ifStat = new IfStatement(expression, body);
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.ElseKey))
            {
                GetNextToken();
                ifStat.ElseBody = ParseBody();
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return ifStat;
        }

        private ReturnStatement ParseReturnStatement()
        {
            GetNextToken();
            return new ReturnStatement(ParseExpression()); // 'return' can return null statement!
        }

        private IPrimaryExpression ParsePrimary()
        {
            var token = PeekCurrentToken();
            switch (token.type)
            {
                case Type.Num:
                    GetNextToken();
                    if (token.value is Int32)    
                        return new IntegerLiteral((int)token.value);
                    return new RealLiteral((double)token.value);
                case Type.True:
                    GetNextToken();
                    return new BooleanLiteral(true);
                case Type.False:
                    GetNextToken();
                    return new BooleanLiteral(false);
                case Type.ThisKey:
                    GetNextToken();
                    return new This();
                case Type.BaseKey:
                    GetNextToken();
                    return new Base();
                case Type.Id:
                    var id = (string)PeekCurrentToken().value;
                    var className = ParseClassName();
                    if (className == null || className.Specification == null || className.Specification.Count == 0)
                    {
                        return new ClassName(id); // should be identifier
                    }
                    return className;
                default:
                    return null;
            }
        }

        private List<Expression> ParseArguments()
        {
            if (!CheckTokenTypeWeak(PeekCurrentToken(), Type.Lparen))
                return null;
            List<Expression> expressions = new List<Expression>();
            GetNextToken();
            
            var expression = ParseExpression();
            if (expression != null) // there is at least one argument in parantheses
            {
                expressions.Add(expression);
                while (CheckTokenTypeWeak(PeekCurrentToken(), Type.Comma))
                {
                    expression = ParseExpression();
                    if (expression == null) // if no parameter after comma
                        throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
                }
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Rparen);
            return expressions;
        }
    }
}