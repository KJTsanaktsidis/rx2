using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regex.Lexer;
using Regex.Parser.Types;

namespace Regex.Parser
{
    public class Parser
    {
        private readonly TokenStream _ts;

        public Parser(TokenStream ts)
        {
            _ts = ts;
        }

        private bool SkipIf(TokenType T)
        {
            if (_ts.Peek().Type == T)
            {
                _ts.NextToken();
                return true;
            }
            return false;
        }

        public RegExpr ParseRegex()
        {
            var re = new RegExpr();
            var follow = new HashSet<TokenType>() 
                {TokenType.EOF, TokenType.CLOSEGROUP};
            do
            {
                re.Alternatives.Add(ParseAlternative());
            } while (SkipIf(TokenType.ALTERNATIVE));

            if (!follow.Contains(_ts.Peek().Type))
                throw new ParseException();
            if (_ts.Peek().Type == TokenType.EOF)
                _ts.NextToken();

            return re;
        }

        private Alternative ParseAlternative()
        {
            var alt = new Alternative();
            var first = new HashSet<TokenType>()
            {
                TokenType.CHAR,
                TokenType.OPENCLASS,
                TokenType.OPENGROUP,
            };
            while (first.Contains(_ts.Peek().Type))
            {
                alt.Factors.Add(ParseMatchOp());
            }
            return alt;
        }

        private SingleChar ParseSingleChar()
        {
            var tk = (CharToken) _ts.NextToken();
            return new SingleChar(tk.Character);
        }

        private Group ParseGroup()
        {
            if (_ts.NextToken().Type != TokenType.OPENGROUP)
                throw new ParseException("Expected (");

            var re = ParseRegex();
            var g = new Group(re);

            if (_ts.NextToken().Type != TokenType.CLOSEGROUP)
                throw new ParseException("Expected )");

            return g;
        }

        private MatchFactorWithOp ParseMatchOp()
        {
            var mf = ParseMatchFactor();
            var op = ParseOp();
            return new MatchFactorWithOp(mf, op);
        }

        private MatchFactor ParseMatchFactor()
        {
            switch (_ts.Peek().Type)
            {
                case TokenType.CHAR:
                    return ParseSingleChar();
                case TokenType.OPENCLASS:
                    return ParseeCClass();
                case TokenType.OPENGROUP:
                    return ParseGroup();
                default:
                    throw new ParseException("Expected cchar, (, [");
            }
        }

        private UnaryOperator ParseOp()
        {
            switch (_ts.Peek().Type)
            {
                case TokenType.OPTIONALOP:
                    _ts.NextToken();
                    return new UnaryOperator(UnaryOperatorType.Optional);
                case TokenType.ONEMANYOP:
                    _ts.NextToken();
                    return new UnaryOperator(UnaryOperatorType.OneMany);
                case TokenType.NONEMANYOP:
                    _ts.NextToken();
                    return new UnaryOperator(UnaryOperatorType.NoneMany);
                default:
                    return new UnaryOperator(UnaryOperatorType.None);
            }
        }

        private CharacterClass ParseeCClass()
        {
            //Throw away [ token
            if (_ts.NextToken().Type != TokenType.OPENCLASS)
                throw new ParseException("Expected [");

            var cClass = new CharacterClass();

            while (_ts.Peek().Type != TokenType.CLOSECLASS)
            {
                cClass.Elements.Add(ParseCClassElement());
            }

            //throw away ]
            _ts.NextToken();

            return cClass;
        }

        private CharacterClassElement ParseCClassElement()
        {
            switch (_ts.Peek().Type)
            {
                case TokenType.CHAR:
                    var tk = _ts.NextToken();
                    return new CharacterClassElement(((CharToken)tk).Character);
                    break;
                default:
                    throw new ParseException("Expected CHAR");
            }
        }
    }
}
