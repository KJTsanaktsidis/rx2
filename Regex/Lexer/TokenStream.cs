using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Lexer
{
    public class TokenStream
    {
        private readonly TextReader _innerStream;
        protected Token Lookahead = null;
        protected IEnumerator<Token> TokenIterator; 

        private enum LexerState
        {
            Normal,
            InEscape
        }

        public TokenStream(TextReader reader)
        {
            _innerStream = reader;
            TokenIterator = NextTokenIterator().GetEnumerator();
        }

        public Token Peek()
        {
            if (Lookahead == null)
            {
                if (!TokenIterator.MoveNext())
                    return null;
                Lookahead = TokenIterator.Current;
            }
            return Lookahead;
        }

        public Token NextToken()
        {
            if (Lookahead != null)
            {
                var lh = Lookahead;
                Lookahead = null;
                return lh;
            }
            else
            {
                if (!TokenIterator.MoveNext())
                    return null;
                return TokenIterator.Current;
            }
        }

        protected IEnumerable<Token> NextTokenIterator()
        {
            int nextCR = _innerStream.Read();
            var state = LexerState.Normal;
            while (nextCR != -1)
            {
                char nextC = (char) nextCR;

                switch (state)
                {
                    case LexerState.Normal:
                        switch (nextC)
                        {
                            case '*':
                                yield return new Token(TokenType.NONEMANYOP);
                                break;
                            case '+':
                                yield return new Token(TokenType.ONEMANYOP);
                                break;
                            case '?':
                                yield return new Token(TokenType.OPTIONALOP);
                                break;
                            case '[':
                                yield return new Token(TokenType.OPENCLASS);
                                break;
                            case ']':
                                yield return new Token(TokenType.CLOSECLASS);
                                break;
                            case '(':
                                yield return new Token(TokenType.OPENGROUP);
                                break;
                            case ')':
                                yield return new Token(TokenType.CLOSEGROUP);
                                break;
                            case '|':
                                yield return new Token(TokenType.ALTERNATIVE);
                                break;
                            case '\\':
                                state = LexerState.InEscape;
                                break;
                            default:
                                yield return new CharToken(nextC);
                                break;
                        }
                        break;
                    case LexerState.InEscape:
                        state = LexerState.Normal;
                        yield return new CharToken(nextC);
                        break;
                }

                nextCR = _innerStream.Read();
            }
            yield return new Token(TokenType.EOF);
        }
    }
}
