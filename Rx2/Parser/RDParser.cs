using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rx2.AST;
using Rx2.CharClassSupport;
using Rx2.Lexer;

namespace Rx2.Parser
{
    public class RDParser
    {
        private readonly IEnumerator<Token> _tokenIterator;

        private Token CurToken { get { return _tokenIterator.Current; } }
        private CharToken CurCharToken { get { return (CharToken) CurToken; }}

        public RDParser(IEnumerable<Token> tokenStream)
        {
            _tokenIterator = tokenStream.GetEnumerator();
            _tokenIterator.MoveNext();
        }

        public RegexNode Parse()
        {
            var re = ParseRegex();
            return re;
        }

        private void AdvanceInput()
        {
            _tokenIterator.MoveNext();
        }

        private void AssertAndAdvance(TokenType tkType)
        {
            if (CurToken.Type != tkType)
                throw new ParseException("Invalid input lookahead");
            AdvanceInput();
        }

        private bool ParseCharClassOpening()
        {
            switch (CurToken.Type)
            {
                case TokenType.OPENCLASS:
                    AdvanceInput();
                    return false;
                case TokenType.OPENCLASSNEGATE:
                    AdvanceInput();
                    return true;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private IEnumerable<CharacterClassElement> ParseCharClassBody()
        {
            var l = new List<CharacterClassElement>();
            ParseCharClassBody(l);
            return l;
        }

        private void ParseCharClassBody(IList<CharacterClassElement> prev)
        {
            switch (CurToken.Type)
            {
                case TokenType.CHAR:
                    prev.Add(ParseCharClassElement());
                    ParseCharClassBody(prev);
                    return;
                case TokenType.CLOSECLASS:
                    return;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private CharacterClassElement ParseCharClassElement()
        {
            var firstc = CurCharToken.Character;
            AdvanceInput();
            var nextc = ParseCharClassRangeM() ?? firstc;
            return new CharacterClassElement(firstc, nextc);
        }

        private char? ParseCharClassRangeM()
        {
            switch (CurToken.Type)
            {
                case TokenType.RANGEMARK:
                    //A-something
                    AdvanceInput();
                    var rangeEndC = CurCharToken.Character;
                    AdvanceInput();
                    return rangeEndC;
                case TokenType.CHAR:
                case TokenType.CLOSECLASS:
                    return null;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private CharacterClass ParseCharClass()
        {
            var cClass = new CharacterClass();
            switch (CurToken.Type)
            {
                case TokenType.OPENCLASS:
                case TokenType.OPENCLASSNEGATE:
                    cClass.IsNegated = ParseCharClassOpening();
                    cClass.Elements.UnionWith(ParseCharClassBody());
                    AssertAndAdvance(TokenType.CLOSECLASS);
                    return cClass;
                case TokenType.CHAR:
                    var curC = CurCharToken.Character;
                    cClass.Elements.Add(new CharacterClassElement(curC, curC));
                    AdvanceInput();
                    return cClass;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private UnaryOperatorType ParseUnaryOp()
        {
            switch (CurToken.Type)
            {
                case TokenType.STAROP:
                    AdvanceInput();
                    return UnaryOperatorType.NoneMany;
                case TokenType.PLUSOP:
                    AdvanceInput();
                    return UnaryOperatorType.OneMany;
                case TokenType.OPTIONALOP:
                    AdvanceInput();
                    return UnaryOperatorType.Optional;
                case TokenType.OPENCLASS:
                case TokenType.OPENCLASSNEGATE:
                case TokenType.OPENGROUP:
                case TokenType.CLOSEGROUP:
                case TokenType.ALTERNATIVE:
                case TokenType.ANY:
                case TokenType.ENDANCHOR:
                case TokenType.CHAR:
                case TokenType.EOF:
                    return UnaryOperatorType.None; //produce e
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private bool ParseGroupOpening()
        {
            switch (CurToken.Type)
            {
                case TokenType.OPENGROUP:
                    AdvanceInput();
                    return true;
                case TokenType.OPENGROUPNOCAP:
                    AdvanceInput();
                    return false;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private MatchFactorNode ParseMatchFactor()
        {
            switch (CurToken.Type)
            {
                case TokenType.OPENCLASS:
                case TokenType.OPENCLASSNEGATE:
                case TokenType.CHAR:
                    var cClass = ParseCharClass();
                    return new CharacterClassMatchNode(cClass);
                case TokenType.OPENGROUP:
                case TokenType.OPENGROUPNOCAP:
                    var shouldCapture = ParseGroupOpening();
                    var group = new GroupMatchNode(capturing: shouldCapture);
                    group.Body = ParseRegexBody();
                    AssertAndAdvance(TokenType.CLOSEGROUP);
                    return group;
                case TokenType.ANY:
                    var anyClass = new CharacterClass();
                    anyClass.Elements.Add(new CharacterClassElement(char.MinValue, char.MaxValue));
                    AdvanceInput();
                    return new CharacterClassMatchNode(anyClass);
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private MatchFactorNode ParseMatchFactorOp()
        {
            switch (CurToken.Type)
            {
                case TokenType.OPENCLASS:
                case TokenType.OPENCLASSNEGATE:
                case TokenType.CHAR:
                case TokenType.OPENGROUP:
                case TokenType.OPENGROUPNOCAP:
                case TokenType.ANY:
                    var mf = ParseMatchFactor();
                    var op = ParseUnaryOp();
                    mf.OpType = op;
                    return mf;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private MatchFactorNode ParseMatchFactorList()
        {
            switch (CurToken.Type)
            {
                case TokenType.OPENCLASS:
                case TokenType.OPENCLASSNEGATE:
                case TokenType.CHAR:
                case TokenType.OPENGROUP:
                case TokenType.OPENGROUPNOCAP:
                case TokenType.ANY:
                    var mf = ParseMatchFactorOp();
                    mf.Next = ParseMatchFactorList();
                    return mf;
                case TokenType.ALTERNATIVE:
                case TokenType.CLOSEGROUP:
                case TokenType.ENDANCHOR:
                case TokenType.EOF:
                    return null;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private AlternativeNode ParseAlternativeRest()
        {
            switch (CurToken.Type)
            {
                case TokenType.ALTERNATIVE:
                    AdvanceInput();
                    var mFactors = ParseMatchFactorList();
                    var af = new AlternativeNode(mFactors);
                    af.Next = ParseAlternativeRest();
                    return af;
                case TokenType.CLOSEGROUP:
                case TokenType.ENDANCHOR:
                case TokenType.EOF:
                    return null;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private RegexNode ParseRegexBody()
        {
            switch (CurToken.Type)
            {
                case TokenType.OPENCLASS:
                case TokenType.OPENCLASSNEGATE:
                case TokenType.CHAR:
                case TokenType.OPENGROUP:
                case TokenType.OPENGROUPNOCAP:
                case TokenType.ANY:
                case TokenType.ALTERNATIVE: //will result in null from ParseMatchFactorList()
                    var mf = ParseMatchFactorList();
                    var af = new AlternativeNode(mf);
                    af.Next = ParseAlternativeRest();
                    return new RegexNode(firstAlternative: af);
                default:
                    throw new ParseException("Invalid input lookahead (I think I need to be more generous here?)");
            }
        }

        private bool ParseStartAnchor()
        {
            switch (CurToken.Type)
            {
                case TokenType.STARTANCHOR:
                    AdvanceInput();
                    return true;
                case TokenType.OPENCLASS:
                case TokenType.OPENCLASSNEGATE:
                case TokenType.CHAR:
                case TokenType.OPENGROUP:
                case TokenType.OPENGROUPNOCAP:
                case TokenType.ANY:
                case TokenType.ALTERNATIVE:
                case TokenType.ENDANCHOR:
                case TokenType.EOF:
                    return false;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private bool ParseEndAnchor()
        {
            switch (CurToken.Type)
            {
                case TokenType.ENDANCHOR:
                    AdvanceInput();
                    return true;
                case TokenType.EOF:
                    return false;
                default:
                    throw new ParseException("Invalid input lookahead");
            }
        }

        private RegexNode ParseRegex()
        {
            //try start/end anchors
            var startAnchor = ParseStartAnchor();
            var reBody = ParseRegexBody();
            var endAnchor = ParseEndAnchor();
            reBody.StartAnchor = startAnchor;
            reBody.EndAnchor = endAnchor;
            AssertAndAdvance(TokenType.EOF);
            return reBody;
        }

    }
}
