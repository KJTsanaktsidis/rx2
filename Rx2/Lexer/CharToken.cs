using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.Lexer
{
    public class CharToken : Token
    {
        public char Character { get; private set; }

        public CharToken(char c) : base(TokenType.CHAR)
        {
            Character = c;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode()/2 + Character.GetHashCode()/2;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj)
                   && obj is CharToken
                   && ((CharToken) obj).Character == Character;
        }
    }
}
