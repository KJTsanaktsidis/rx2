using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser.Types
{
    public class SingleChar : MatchFactor
    {
        public char Character { get; private set; }

        public SingleChar(char c)
        {
            Character = c;
        }
    }
}
