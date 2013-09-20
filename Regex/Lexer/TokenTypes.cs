using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Lexer
{
    public enum TokenType
    {
        CHAR,
        NONEMANYOP,
        ONEMANYOP,
        OPTIONALOP,
        OPENCLASS,
        CLOSECLASS,
        OPENGROUP,
        CLOSEGROUP,
        ALTERNATIVE,
        EOF
    }
}
