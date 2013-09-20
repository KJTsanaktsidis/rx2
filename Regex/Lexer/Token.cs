using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Lexer
{
    public class Token
    {
        public TokenType Type { get; protected set; }

        public Token(TokenType type)
        {
            Type = type;
        }
    }
}
