using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.Automaton
{
    public class NFAState
    {
        public int Label { get; private set; }
        public IList<FAStateTags> Tags { get; set; }

        public NFAState(int label)
        {
            Label = label;
            Tags = new List<FAStateTags>();
        }

        public override string ToString()
        {
            return "S" + Label;
        }
    }

    public enum FAStateTags
    {
        None,
        Start,
        Final,
        PushSubmatch,
        PopSubmatch
    }
}
