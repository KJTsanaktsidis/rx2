using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rx2.Lexer
{
    public class TokenStream : IEnumerable<Token>
    {
        private readonly TextReader _innerStream;
        protected Token Lookahead = null;

        private enum LexerState
        {
            Normal,
            OpeningCharClass,
            CharClass,
            OpeningGroup,
            OpeningGroup2,
            Escape
        }

        public TokenStream(TextReader reader)
        {
            _innerStream = reader;
        }

        protected IEnumerable<Token> NextTokenIterator()
        {
            int nextCR = _innerStream.Read();
            var state = LexerState.Normal;
            var oldEscapeState = LexerState.Normal;
            while (nextCR != -1)
            {
                char nextC = (char) nextCR;
                char? lookahead = _innerStream.Peek() == -1 ? null : (char?) _innerStream.Peek();

                switch (state)
                {
                    case LexerState.Normal:
                        switch (nextC)
                        {
                            case  '*':
                                yield return new Token(TokenType.STAROP);
                                break;
                            case '+':
                                yield return new Token(TokenType.PLUSOP);
                                break;
                            case '?':
                                yield return new Token(TokenType.OPTIONALOP);
                                break;
                            case ')':
                                yield return new Token(TokenType.CLOSEGROUP);
                                break;
                            case '|':
                                yield return new Token(TokenType.ALTERNATIVE);
                                break;
                            case '.':
                                yield return new Token(TokenType.ANY);
                                break;
                            case '^':
                                yield return new Token(TokenType.STARTANCHOR);
                                break;
                            case '$':
                                yield return new Token(TokenType.ENDANCHOR);
                                break;
                            case '[':
                                if (lookahead == '^')
                                    state = LexerState.OpeningCharClass;
                                else
                                {
                                    yield return new Token(TokenType.OPENCLASS);
                                    state = LexerState.CharClass;
                                }
                                break;
                            case '(':
                                if (lookahead == '?')
                                    state = LexerState.OpeningGroup;
                                else
                                    yield return new Token(TokenType.OPENGROUP);
                                break;
                            case '\\':
                                oldEscapeState = state;
                                state = LexerState.Escape;
                                break;
                            default:
                                yield return new CharToken(nextC);
                                break;
                        }
                        break;
                    case LexerState.OpeningCharClass:
                        //must have had a ^ to get ehre
                        yield return new Token(TokenType.OPENCLASSNEGATE);
                        state = LexerState.CharClass;
                        break;
                    case LexerState.OpeningGroup:
                        if (lookahead == ':')
                            state = LexerState.OpeningGroup2;
                        else
                        {
                            yield return new Token(TokenType.OPENGROUP); //for the (
                            state = LexerState.Normal;
                        }
                        break;
                    case LexerState.OpeningGroup2:
                        //must have had a ?: to get here
                        yield return new Token(TokenType.OPENGROUPNOCAP);
                        state = LexerState.Normal;
                        break;
                    case LexerState.CharClass:
                        switch (nextC)
                        {
                            case ']':
                                yield return new Token(TokenType.CLOSECLASS);
                                state = LexerState.Normal;
                                break;
                            case '\\':
                                oldEscapeState = state;
                                state = LexerState.Escape;
                                break;
                            case '-':
                                yield return new Token(TokenType.RANGEMARK);
                                break;
                            default:
                                yield return new CharToken(nextC);
                                break;
                        }
                        break;
                    case LexerState.Escape:
                        yield return new CharToken(nextC);
                        state = oldEscapeState;
                        break;

                }

                nextCR = _innerStream.Read();
            }
            yield return new Token(TokenType.EOF);
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return NextTokenIterator().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}


