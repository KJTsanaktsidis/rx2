using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.Automaton
{
    public class DFAState : IEquatable<DFAState>
    {
        private readonly IList<FAStateTags> _stateTags; 
        private readonly IList<NFAState> _orderedConstituents; 

        public ISet<NFAState> Constituents { get; private set; }
        public IList<FAStateTags> Tags {get { return new ReadOnlyCollection<FAStateTags>(_stateTags);}} 

        public DFAState(IList<NFAState> constituents)
        {
            Constituents = new HashSet<NFAState>(constituents);
            _orderedConstituents = constituents;
            _stateTags = constituents.SelectMany(s => s.Tags).ToList();
        }


        public override bool Equals(object obj)
        {
            return obj is DFAState
                   && Equals(obj as DFAState);
        }

        public override int GetHashCode()
        {
            int hc = Constituents.Aggregate(0, (ac, state) => ac += state.GetHashCode());
            return hc;
        }

        public bool Equals(DFAState other)
        {
            return other.Constituents.SetEquals(Constituents);
        }

        public override string ToString()
        {
            return string.Join(", ", _orderedConstituents.Select(c => c.ToString()));
        }
    }
}
