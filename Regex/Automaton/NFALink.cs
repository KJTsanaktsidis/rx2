using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regex.Parser.Types;

namespace Regex.Automaton
{
    public class NFALink
    {
        public CharacterClass CharClass { get; set; } 
        public bool IsEmpty { get; set; }
        public NFAState Target { get; set; }

        public override string ToString()
        {
            return "(" + CharClass.ToString() + ", " + Target.ToString().Substring(0, 3) + ")";
        }
    }
}
