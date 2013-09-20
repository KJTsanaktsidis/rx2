using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Lexer
{
    public class CharToken : Token
    {
        public char Character { get; private set; }

        public CharToken(char c) : base(TokenType.CHAR)
        {
            Character = c;
        }
    }
}
