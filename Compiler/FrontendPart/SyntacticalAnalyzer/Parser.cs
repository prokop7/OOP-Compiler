using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
        private List<Token> tokensList;
        private List<Token>.Enumerator tokensListIterator;
        private Lexer lexer;
        private Token currentToken = null;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }

        private Token GetNextToken()
        {
            currentToken = lexer.GetNextToken();
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
            {
                var lastToken = tokensList.Last();
                throw new UnexpectedTokenException(lastToken.lineNumber, lastToken.positionNumber, Token.StringValueOf(expectedType) + expectation);
            }
            if(!token.type.Equals(expectedType))
            {
                throw new UnexpectedTokenException(token.lineNumber, token.positionNumber, Token.StringValueOf(expectedType) + expectation);
            }
        }
        
        /* Checks optional tokens */
        private bool CheckTokenTypeWeak(Token token, Type expectedType)
        {
            if (token == null)
            {
                var lastToken = tokensList.Last();
                throw new UnexpectedTokenException(lastToken.lineNumber, lastToken.positionNumber);
            }
            return token.type.Equals(expectedType);
        }
        
        private Class ParseClass()
        {
            if (GetNextToken() == null)
            {
                return null;
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.ClassKey);
            var classObj = new Class(ParseClassName());
            if (CheckTokenTypeWeak(GetNextToken(), Type.ExtendsKey))
            {
                classObj.BaseClassName = ParseClassName();
            }
            CheckTokenTypeStrong(GetNextToken(), Type.IsKey);

            /* Parse body of class */
            IMemberDeclaration memberDeclaration;
            while ((memberDeclaration = ParseMemberDeclaration()) != null)
            {
                classObj.MemberDeclarations.Add(memberDeclaration);
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return classObj;
        }

        private ClassName ParseClassName()
        {
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Id, " of class name");
            var className = new ClassName((string)PeekCurrentToken().value);
            
            var genericParams = ParseGenericParams();
            if (genericParams != null)
            {
                className.Specification = genericParams;
            }
            return className;

        }

        private List<string> ParseGenericParams()
        {
            if (!CheckTokenTypeWeak(GetNextToken(), Type.SqrtLparen))
            {
                return null;
            }
            List<string> genericParams = new List<string>();
            do
            {
                CheckTokenTypeStrong(GetNextToken(), Type.Id);
                genericParams.Add((string)PeekCurrentToken().value);
            } while (CheckTokenTypeWeak(GetNextToken(), Type.Comma));
            CheckTokenTypeStrong(PeekCurrentToken(), Type.SqrtRparen);
            
            return genericParams;
        }

        private IMemberDeclaration ParseMemberDeclaration()
        {
            switch (PeekCurrentToken().type)
            {
                case Type.EndKey:
                {
                    return null;
                }
                case Type.VarKey:
                {
                    return ParseVariableDeclaration();
                }
                case Type.MethodKey:
                {
                    return ParseMethodDeclaration();
                }
                case Type.ThisKey:
                {
                    return ParseConstructorDeclaration();
                }
                default:
                {
                    throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
                }
            }
        }

        private VariableDeclaration ParseVariableDeclaration()
        {
            CheckTokenTypeStrong(GetNextToken(), Type.Id, " of variable name");
            var varName = (string)PeekCurrentToken().value;
            string className = null;
            if (CheckTokenTypeWeak(GetNextToken(), Type.Colon))
            {
                CheckTokenTypeStrong(GetNextToken(), Type.Id, " of class name");
                className = (string)PeekCurrentToken().value;
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.IsKey);
            tokensListIterator.MoveNext();
            var expression = ParseExpression();
            if (expression == null)
            {
                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "expression");
            }
            var varDeclaration = new VariableDeclaration(varName, expression);
            if (className != null)
            {
                varDeclaration.ClassName = className;
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
                method.Parameters = parameters;
            }
            if (CheckTokenTypeWeak(GetNextToken(), Type.Colon))
            {
                CheckTokenTypeStrong(GetNextToken(), Type.Id, " of return value type");
            }

            var body = ParseBody();
            method.Body = body;
            
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);

            return method;
        }

        private ConstructorDeclaration ParseConstructorDeclaration()
        {
            var constructor = new ConstructorDeclaration();
            var parameters = ParseParameters();
            if (parameters != null)
            {
                constructor.Parameters = parameters;
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.IsKey);
            var body = ParseBody();
            constructor.Body = body;
            
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return constructor;
        }

        private Expression ParseExpression()
        {
            var primary = ParsePrimary();
            if (primary == null)
            {
                return null;
            }
            List<MethodOrFieldCall> calls = new List<MethodOrFieldCall>();
            while (CheckTokenTypeWeak(GetNextToken(), Type.Dot))
            {
                CheckTokenTypeStrong(GetNextToken(), Type.Id);
                var id = (string)PeekCurrentToken().value;
                var arguments = ParseArguments();
                calls.Add(new MethodOrFieldCall(id, arguments));
            }
            return new Expression(primary, calls);
        }

        private List<ParameterDeclaration> ParseParameters()
        {
            if (!CheckTokenTypeWeak(GetNextToken(), Type.Lparen)) // if method declaration without empty parantheses
            {
                return null;
            }
            List<ParameterDeclaration> parameters = new List<ParameterDeclaration>();
            var parameter = ParseParameterDeclaration();
            if (parameter != null) // there is at least one parameter in parantheses
            {
                parameters.Add(parameter);
                while (CheckTokenTypeWeak(GetNextToken(), Type.Comma))
                {
                    parameter = ParseParameterDeclaration();
                    if (parameter == null) // if no parameter after comma
                    {
                        throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
                    }
                }
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Rparen);
            return parameters;
        }

        private ParameterDeclaration ParseParameterDeclaration()
        {
            if (!CheckTokenTypeWeak(GetNextToken(), Type.Id))
            {
                return null;
            }
            var paramName = (string)PeekCurrentToken().value;
            CheckTokenTypeStrong(GetNextToken(), Type.Colon);
            var className = ParseClassName();
            return new ParameterDeclaration(paramName, className);
        }

        private List<IBody> ParseBody()
        {
            List<IBody> body = new List<IBody>();
            while (!CheckTokenTypeWeak(GetNextToken(), Type.EndKey))
            {
                switch (PeekCurrentToken().type)
                {
                    case Type.VarKey:
                    {
                        body.Add(ParseVariableDeclaration());
                        break;
                    }
                    case Type.Id:
                    {
                        body.Add(ParseAssignment());
                        break;
                    }
                    case Type.WhileKey:
                    {
                        body.Add(ParseWhileLoop());
                        break;
                    }
                    case Type.IfKey:
                    {
                        body.Add(ParseIfStatement());
                        break;
                    }
                    case Type.ReturnKey:
                    {
                        body.Add(ParseReturnStatement());
                        break;
                    }
                    default:
                    {
                        throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
                    }
                }
            }
            return body;
        }

        private Assignment ParseAssignment()
        {
            CheckTokenTypeStrong(GetNextToken(), Type.Id);
            var id = (string)PeekCurrentToken().value;
            CheckTokenTypeStrong(GetNextToken(), Type.Assignment);
            tokensListIterator.MoveNext();
            var expression = ParseExpression();
            if (expression == null)
            {
                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "expression");
            }
            return new Assignment(id, expression);
        }

        private WhileLoop ParseWhileLoop()
        {
            tokensListIterator.MoveNext();
            var expression = ParseExpression();
            if (expression == null)
            {
                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "expression");
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.LoopKey);
            var body = ParseBody();
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return new WhileLoop(expression, body);
        }

        private IfStatement ParseIfStatement()
        {
            tokensListIterator.MoveNext();
            var expression = ParseExpression();
            if (expression == null)
            {
                throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber, "expression");
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.ThenKey);
            var body = ParseBody();
            var ifStat = new IfStatement(expression, body);
            if (CheckTokenTypeWeak(PeekCurrentToken(), Type.ElseKey))
            {
                ifStat.ElseBody = ParseBody();
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.EndKey);
            return ifStat;
        }

        private ReturnStatement ParseReturnStatement()
        {
            tokensListIterator.MoveNext();
            return new ReturnStatement(ParseExpression());
        }

        private IPrimaryExpression ParsePrimary()
        {
            var token = PeekCurrentToken();
            switch (token.type)
            {
                case Type.Num:
                {
                    if ((long)token.value != null)
                    {
                        return new IntegerLiteral((int)token.value);
                    }
                    return new RealLiteral((double)token.value);
                }
                case Type.True:
                {
                    return new BooleanLiteral(true);
                }
                case Type.False:
                {
                    return new BooleanLiteral(false);
                }
                case Type.ThisKey:
                {
                    return new This();
                }
                case Type.BaseKey:
                {
                    return new Base();
                }
                case Type.Id:
                {
                    return ParseClassName();
                }
                default:
                {
                    return null;
                }
            }
        }

        private List<Expression> ParseArguments()
        {
            if (!CheckTokenTypeWeak(GetNextToken(), Type.Lparen))
            {
                return null;
            }
            List<Expression> expressions = new List<Expression>();
            tokensListIterator.MoveNext();
            
            var expression = ParseExpression();
            if (expression != null) // there is at least one argument in parantheses
            {
                expressions.Add(expression);
                while (CheckTokenTypeWeak(GetNextToken(), Type.Comma))
                {
                    expression = ParseExpression();
                    if (expression == null) // if no parameter after comma
                    {
                        throw new UnexpectedTokenException(PeekCurrentToken().lineNumber, PeekCurrentToken().positionNumber);
                    }
                }
            }
            CheckTokenTypeStrong(PeekCurrentToken(), Type.Rparen);
            return expressions;
        }
    }
}