using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.Lexer
{
    public class Token
    {
        public TokenType Type { get; private set; }

        public Token(TokenType type)
        {
            Type = type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var otype = obj as Token;
            if (otype == null)
                return false;
            return Type == otype.Type;
        }
    }
}
