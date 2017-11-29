using System;
using System.Collections.Generic;
using System.Linq;
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
//        private Token currentToken = null;
        private Token prevToken = null;
        private List<Token> tokensToProcess = new List<Token>();
        

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }

        private Token GetNextToken()
        {
            if (tokensToProcess.Count > 0)
            {
                prevToken = tokensToProcess.First();
                tokensToProcess.RemoveAt(0);
            }
            if (tokensToProcess.Count > 0)
            {
                return tokensToProcess.First();
            }
            var token = lexer.GetNextToken();
            if (token == null)
                return null;
            tokensToProcess.Add(token);
            return tokensToProcess.First();
        }

        private Token PeekCurrentToken()
        {
            return tokensToProcess.First();
//            return currentToken;
        }

        private void ReturnTokensToProcess(List<Token> tokens)
        {
            tokensToProcess.InsertRange(0, tokens);
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
                throw new UnexpectedTokenException(prevToken, Token.StringValueOf(expectedType) + expectation);
            if(!token.type.Equals(expectedType))
                throw new UnexpectedTokenException(token, Token.StringValueOf(expectedType) + expectation);
        }
        
        /* Checks optional tokens */
        private bool CheckTokenTypeWeak(Token token, Type expectedType)
        {
            if (token == null)
                throw new UnexpectedTokenException(prevToken);
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
                    throw new UnexpectedTokenException(PeekCurrentToken(), "base class name");
                baseClassName.Parent = classObj;
                classObj.BaseClassName = baseClassName;
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.IsKey);

            /* Parse body of class */
            IMemberDeclaration memberDeclaration;
            GetNextToken();
            while ((memberDeclaration = ParseMemberDeclaration()) != null)
            {
                memberDeclaration.Parent = classObj;
                classObj.MemberDeclarations.Add(memberDeclaration);
//                GetNextToken();
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return classObj;
        }

        private ClassName ParseClassName()
        {
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Id, " of class name");
            var className = new ClassName((string)PeekCurrentToken().value);

            GetNextToken();
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.TrglLparen))
            {
                var genericParams = ParseGenericParams();
                if (genericParams.Count == 0)
                    throw new UnexpectedTokenException(PeekCurrentToken(), "generic parameter");
                foreach (var genericParam in genericParams)
                {
                    genericParam.Parent = className;
                    className.Specification.Add(genericParam);
                }
                CheckTokenTypeStrong(PeekCurrentToken(), Type.TrglRparen);
                GetNextToken();
            }
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.SqrtLparen))
            {
                GetNextToken();
                CheckTokenTypeStrong(PeekCurrentToken(), Type.Num);
                className.ArrSize = (int) PeekCurrentToken().value;
                CheckTokenTypeStrong(GetNextToken(), Type.SqrtRparen);
                GetNextToken();
            }

            return className;

        }

        private List<ClassName> ParseGenericParams()
        {
            var genericParams = new List<ClassName>();
            do
            {
                GetNextToken();
                var param = ParseClassName();
                if (param == null)
                    throw new UnexpectedTokenException(PeekCurrentToken(), "class name");
                genericParams.Add(param);
            } while (CheckTokenTypeWeak(PeekCurrentToken(), Type.Comma));
            return genericParams;
        }

        private IMemberDeclaration ParseMemberDeclaration()
        {
            switch (PeekCurrentToken().type)
            {
                case Type.EndKey:
                    return null;
                case Type.VarKey:
                    GetNextToken();
                    return ParseVariableDeclaration();
                case Type.MethodKey:
                    return ParseMethodDeclaration();
                case Type.ThisKey:
                    return ParseConstructorDeclaration();
                default:
                    throw new UnexpectedTokenException(PeekCurrentToken());
            }
        }

        private VariableDeclaration ParseVariableDeclaration()
        {
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Id, " of variable name");
            var varName = (string)PeekCurrentToken().value;
            var varDeclaration = new VariableDeclaration(varName);
            ClassName className = null;
            var typeOrValueDefined = false;
            if (CheckTokenTypeWeak(GetNextToken(), Type.Colon))
            {
                GetNextToken();
                className = ParseClassName();
                if (className == null)
                    throw new UnexpectedTokenException(PeekCurrentToken(), "class name");
                className.Parent = varDeclaration;
                varDeclaration.Classname = className;
                varDeclaration.Expression = new Expression(new ClassName(className) {Parent = varDeclaration});
                varDeclaration.IsDeclared = false;
                typeOrValueDefined = true;
            }
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.IsKey))
            {
                GetNextToken();
                var expression = ParseExpression();
                if (expression == null)
                    throw new UnexpectedTokenException(PeekCurrentToken(), "expression");
                expression.Parent = varDeclaration;
                varDeclaration.Expression = expression;
                varDeclaration.IsDeclared = true;
                typeOrValueDefined = true;
            }
            if(!typeOrValueDefined)
                throw new UnexpectedTokenException(PeekCurrentToken(), $"explisit type or value of variable '{varName}'");
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
                GetNextToken();
                var resultType = ParseClassName();
                if(resultType == null)
                    throw new UnexpectedTokenException(PeekCurrentToken(), "class name");
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
            GetNextToken();
            return method;
        }

        private ConstructorDeclaration ParseConstructorDeclaration()
        {
            var parameters = ParseParameters();
            CheckTokenTypeStrong(PeekCurrentToken(), Type.IsKey);
            GetNextToken();
            var body = ParseBody();
            
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            GetNextToken();
            return new ConstructorDeclaration(parameters, body);
        }

        private Expression ParseExpression()
        {
            var primary = ParsePrimary();
            if (primary == null)
                return null;
            var calls = new List<ICall>();
            while(CheckTokenTypeWeak(PeekCurrentToken(), Type.Dot))
            {
                CheckTokenTypeStrong(GetNextToken(), Type.Id);
                var call = ParseCall();
                if(call == null)
                    throw new UnexpectedTokenException(PeekCurrentToken(), "method or field call");    
                calls.Add((ICall)call);
            }
            if(calls.Count == 0)
                return new Expression(primary);
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
                while (CheckTokenTypeWeak(PeekCurrentToken(), Type.Comma))
                {
                    parameter = ParseParameterDeclaration();
                    if (parameter == null) // if no parameter after comma
                        throw new UnexpectedTokenException(PeekCurrentToken());
                }
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Rparen);
            GetNextToken();
            return parameters;
        }

        private ParameterDeclaration ParseParameterDeclaration()
        {
            if (!CheckTokenTypeWeak(GetNextToken(), Type.Id))
                return null;
            var paramName = (string)PeekCurrentToken().value;
            CheckTokenTypeStrong(GetNextToken(), Type.Colon);
            GetNextToken();
            var className = ParseClassName();
            if (className == null)
                throw  new UnexpectedTokenException(PeekCurrentToken(), "class name");
            return new ParameterDeclaration(paramName, className);
        }

        private List<IBody> ParseBody()
        {
            List<IBody> body = new List<IBody>();
            while (!CheckTokenTypeWeak(PeekCurrentToken(), Type.EndKey) && !CheckTokenTypeWeak(PeekCurrentToken(), Type.ElseKey))
            {
                switch (PeekCurrentToken().type)
                {
                    case Type.VarKey:
                        GetNextToken();
                        body.Add(ParseVariableDeclaration());
                        break;
                    case Type.Id:
                        var firstToken = PeekCurrentToken();
                        var id = (string)PeekCurrentToken().value;
                        if (CheckTokenTypeWeak(GetNextToken(), Type.Assignment))
                        {
                            GetNextToken();
                            var expression = ParseExpression();
                            if (expression == null)
                                throw new UnexpectedTokenException(PeekCurrentToken(), "expression");
                            body.Add(new Assignment(id, expression));
                            break;
                        }
                        ReturnTokensToProcess(new List<Token>(){firstToken});
                        var expr = ParseExpression();
                        if(expr == null)
                            throw new UnexpectedTokenException(PeekCurrentToken());
                        body.Add(new Expression(expr));
                        break;
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
                        break;
                    default:
                        throw new UnexpectedTokenException(PeekCurrentToken());
                }
            }
            return body;
        }

        private WhileLoop ParseWhileLoop()
        {
            GetNextToken();
            var expression = ParseExpression();
            if (expression == null)
                throw new UnexpectedTokenException(PeekCurrentToken(), "expression");
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
                throw new UnexpectedTokenException(PeekCurrentToken(), "expression");
            CheckTokenTypeStrong(PeekCurrentToken(), Type.ThenKey);
            GetNextToken();
            var body = ParseBody();
            var ifStat = new IfStatement(expression, body);
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.ElseKey))
            {
                GetNextToken();
                ifStat.ElseBody = ParseBody();
                ifStat.ElseBody.ForEach(el => el.Parent = ifStat);
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return ifStat;
        }

        private ReturnStatement ParseReturnStatement()
        {
            GetNextToken();
            return new ReturnStatement(ParseExpression()); // 'return' can return null statement!
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
                    GetNextToken();
                    expression = ParseExpression();
                    if (expression == null) // if no parameter after comma
                        throw new UnexpectedTokenException(PeekCurrentToken());
                }
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Rparen);
            return expressions;
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
                    return (IPrimaryExpression)ParseCall(true);
                default:
                    return null;
            }
        }

        private ICommonCall ParseCall(bool primary = false)
        {
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Id);
            string id = null;
            ClassName className = null;
            if (primary)
            {
                className = ParseClassName();
                if (className.ArrSize == null && !className.Specification.Any())
                    id = className.Identifier;
                else // if constructor of array of objects
                    CheckTokenTypeStrong(PeekCurrentToken(), Type.Lparen);
            }
            else
            {
                id = (string) PeekCurrentToken().value;
                GetNextToken();
            }
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.Lparen))
            {
                var args = ParseArguments();
                GetNextToken();
                if(!primary)
                    return new Call(id, args);
                if (id != null)
                    return new LocalCall(id, args);
                return new ConstructorCall(className, args);
            }
            if(primary)
                return new LocalCall(id);
            return new FieldCall(id);
        }
    }
}