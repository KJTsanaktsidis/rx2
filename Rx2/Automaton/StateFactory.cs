using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.Automaton
{
    public class StateFactory
    {
        public int CurrentIndex { get; private set; }

        public NFAState MakeState()
        {
            var s = new NFAState(CurrentIndex);
            CurrentIndex++;
            return s;
        }
    }
}
