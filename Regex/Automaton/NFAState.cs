using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Automaton
{
    public class NFAState
    {
        public override string ToString()
        {
            return base.GetHashCode().ToString();
        }
    }
}
