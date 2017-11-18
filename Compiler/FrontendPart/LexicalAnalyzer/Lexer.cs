using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Exceptions;
using OverflowException = Compiler.Exceptions.OverflowException;

namespace Compiler.FrontendPart.LexicalAnalyzer
{
    public class Lexer
    {
        private FileScanner fileScanner;

        private List<Token> Tokens { get; }

        private int currentLine;
        private int currentPosition;
        private string line;

        public Lexer(string fileName)
        {
            fileScanner = new FileScanner(fileName);
            Tokens = new List<Token>();
            currentLine = 0;
            currentPosition = 0;
        }

        public Token GetNextToken()
        {
            if (Tokens.Count > 0)
            {
                return PullFirstToken();
            }

            ScanNextTokens();
            if (Tokens.Count == 0)
            {
                return null;
            }
            return PullFirstToken();
        }

        /* returns first token from 'Tokens' with deleting it from the list  */
        private Token PullFirstToken()
        {
            var token = Tokens.First();
            Tokens.RemoveAt(0);
            return token;
        }

        private void ScanNextTokens()
        {
            while (Tokens.Count == 0)
            {
                if ((line = fileScanner.ReadLine()) == null)
                {
                    return;
                }
                currentLine++;
                currentPosition = 0;

                while (currentPosition < line.Length)
                {
                    SkipSpaces();
                    if (line.Length <= currentPosition)
                    {
                        continue;
                    }
                    var lexeme = GetNextLexeme(line, currentPosition);

                    switch (lexeme)
                    {
                        case "/":
                        {
                            if (currentPosition + 1 < line.Length && line[currentPosition + 1] == '/')
                            {
                                currentPosition = line.Length;
                                break;
                            }
                            throw new UnexpectedDigitException(currentLine, currentPosition);
                        }
                        case "true":
                        {
                            Tokens.Add(new Token(Type.True, currentLine, currentPosition));
                            break;
                        }
                        case "false":
                        {
                            Tokens.Add(new Token(Type.False, currentLine, currentPosition));
                            break;
                        }
                        case ":":
                        {
                            if (currentPosition + 1 < line.Length && line[currentPosition + 1] == '=')
                            {
                                Tokens.Add(new Token(Type.Assignment, currentLine, currentPosition));
                                currentPosition++;
                                break;
                            }
                            Tokens.Add(new Token(Type.Colon, currentLine, currentPosition));
                            break;
                        }
                        case ",":
                        {
                            Tokens.Add(new Token(Type.Comma, currentLine, currentPosition));
                            break;
                        }
                        case ".":
                        {
                            Tokens.Add(new Token(Type.Dot, currentLine, currentPosition));
                            break;
                        }
                        case "(":
                        {
                            Tokens.Add(new Token(Type.Lparen, currentLine, currentPosition));
                            break;
                        }
                        case ")":
                        {
                            Tokens.Add(new Token(Type.Rparen, currentLine, currentPosition));
                            break;
                        }
                        case "[":
                        {
                            Tokens.Add(new Token(Type.SqrtLparen, currentLine, currentPosition));
                            break;
                        }
                        case "]":
                        {
                            Tokens.Add(new Token(Type.SqrtRparen, currentLine, currentPosition));
                            break;
                        }
                        case "is":
                        {
                            Tokens.Add(new Token(Type.IsKey, currentLine, currentPosition));
                            break;
                        }
                        case "if":
                        {
                            Tokens.Add(new Token(Type.IfKey, currentLine, currentPosition));
                            break;
                        }
                        case "var":
                        {
                            Tokens.Add(new Token(Type.VarKey, currentLine, currentPosition));
                            break;
                        }
                        case "end":
                        {
                            Tokens.Add(new Token(Type.EndKey, currentLine, currentPosition));
                            break;
                        }
                        case "else":
                        {
                            Tokens.Add(new Token(Type.ElseKey, currentLine, currentPosition));
                            break;
                        }
                        case "extends":
                        {
                            Tokens.Add(new Token(Type.ExtendsKey, currentLine, currentPosition));
                            break;
                        }
                        case "this":
                        {
                            Tokens.Add(new Token(Type.ThisKey, currentLine, currentPosition));
                            break;
                        }
                        case "then":
                        {
                            Tokens.Add(new Token(Type.ThenKey, currentLine, currentPosition));
                            break;
                        }
                        case "loop":
                        {
                            Tokens.Add(new Token(Type.LoopKey, currentLine, currentPosition));
                            break;
                        }
                        case "base":
                        {
                            Tokens.Add(new Token(Type.BaseKey, currentLine, currentPosition));
                            break;
                        }
                        case "class":
                        {
                            Tokens.Add(new Token(Type.ClassKey, currentLine, currentPosition));
                            break;
                        }
                        case "while":
                        {
                            Tokens.Add(new Token(Type.WhileKey, currentLine, currentPosition));
                            break;
                        }
                        case "method":
                        {
                            Tokens.Add(new Token(Type.MethodKey, currentLine, currentPosition));
                            break;
                        }
                        case "return":
                        {
                            Tokens.Add(new Token(Type.ReturnKey, currentLine, currentPosition));
                            break;
                        }
                        default:
                        {
                            if (Char.IsLetter(lexeme[0]))
                            {
                                Tokens.Add(new Token(Type.Id, currentLine, currentPosition, lexeme));
                                break;
                            }
                            if (Char.IsDigit(lexeme[0]))
                            {
                                Tokens.Add(new Token(Type.Num, currentLine, currentPosition, ParseNumber(lexeme)));
                                break;
                            }
                            throw new UnexpectedDigitException(currentLine,currentPosition);
                        }

                    }
                    currentPosition += lexeme.Length;
                }
            }
        }

        private void SkipSpaces()
        {
            while (currentPosition < line.Length && (line[currentPosition] == ' ' || line[currentPosition] == '\t'))
                currentPosition++;
        }

        private string GetNextLexeme(string line, int startPosition)
        {
            var lexeme = line.Substring(startPosition, 1);
            if(!Char.IsLetterOrDigit(line[startPosition]))
            {
                return lexeme;
            }
            var position = startPosition + 1;
            /* if keyword or identifier */
            if (Char.IsLetter(line[startPosition]))
            {
                while (position < line.Length && (Char.IsLetterOrDigit(line[position]) || line[position] == '_'))
                {
                    lexeme += line[position++];
                }
                return lexeme;
            }
            /* if the first char is digit */
            while (position < line.Length && Char.IsDigit(line[position]))
            {
                lexeme += line[position++];
            }
            if (position + 1 < line.Length && line[position] == '.' && Char.IsDigit(line[position + 1]))
            {
                lexeme += line[position++];
                while (position < line.Length && Char.IsDigit(line[position]))
                {
                    lexeme += line[position++];
                }
            }
                if (position < line.Length && Char.IsLetter(line[position]))
                {
                    throw new UnexpectedDigitException(currentLine, currentPosition);
                }
            return lexeme;

        }

        private object ParseNumber(string lexeme)
        {
            if (!lexeme.Contains("."))
            {
                return ParseInteger(lexeme);
            }
            return ParseReal(lexeme);
        }

        private int ParseInteger(string lexeme)
        {
            long result = 0;
            for (int i = 0; i < lexeme.Length; i++)
            {
                result = result * 10 + lexeme[i] - '0';
                if (Int32.MaxValue < result)
                {
                    throw new OverflowException(currentLine, currentPosition);
                }
            }
            return Convert.ToInt32(result);
        }

        private double ParseReal(string lexeme)
        {
            double result = 0.0;
            long intPart = 0;
            int pointIndex = lexeme.IndexOf('.');
            long divider = 1;
            for (int i = pointIndex + 1; i < lexeme.Length; i++)
            {
                divider *= 10;
                result = result + (double)(lexeme[i] - '0') / divider;
                if (float.MaxValue < result)
                {
                    throw new OverflowException(currentLine, currentPosition);
                }
            }
            for (int i = 0; i < pointIndex; i++)
            {
                intPart = intPart * 10 + (int)(lexeme[i]) - '0';
            }
            result += intPart;

            return result;
        }

    }
}