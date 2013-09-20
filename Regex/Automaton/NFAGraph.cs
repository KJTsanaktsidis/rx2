using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Regex.Parser.Types;

namespace Regex.Automaton
{
    public class NFAGraph
    {
        private ISet<NFAState> _nodes = new HashSet<NFAState>();
        private IDictionary<NFAState, IList<NFALink>> _adjList = new Dictionary<NFAState, IList<NFALink>>();
        private NFAState _startState = null;
        private HashSet<NFAState> _finishState = new HashSet<NFAState>();

        public NFAGraph(CharacterClass CClass)
        {
            InstantiateFromCharClass(CClass);
        }

        public NFAGraph(SingleChar C)
        {
            InstantiateFromSingleChar(C);
        }

        public NFAGraph(MatchFactor mtch)
        {
            InstantiateFromMatchFactor(mtch);
        }

        public NFAGraph(MatchFactorWithOp mtch)
        {
            InstantiateFromMatchFactorOp(mtch);
        }

        public NFAGraph(Alternative alt)
        {
            InstantiateFromAlternative(alt);
        }

        public NFAGraph(RegExpr reParseTree)
        {
            InstantiateFromRegex(reParseTree);
        }

        public NFAGraph()
        {
            //top down construction
        }

        private void InstantiateFromRegex(RegExpr re)
        {
            _startState = NewState();
            _finishState.Add(NewState());
            foreach (var alt in re.Alternatives)
            {
                var altnfa = new NFAGraph(alt);
                CopyInto(altnfa);

                var nsl = new NFALink();
                var nfl = new NFALink();
                nsl.IsEmpty = true;
                nfl.IsEmpty = true;
                nsl.Target = altnfa._startState;
                nfl.Target = _finishState.Single();
                _adjList[_startState].Add(nsl);
                _adjList[altnfa._finishState.Single()].Add(nfl);
            }
        }

        private void InstantiateFromAlternative(Alternative alt)
        {
            _startState = NewState();
            _finishState.Add(NewState());
            var curtail = _startState;
            foreach (var mf in alt.Factors)
            {
                var mfnfa = new NFAGraph(mf);
                CopyInto(mfnfa);

                var nl = new NFALink();
                nl.IsEmpty = true;
                nl.Target = mfnfa._startState;
                _adjList[curtail].Add(nl);
                curtail = mfnfa._finishState.Single();
            }
            var nel = new NFALink();
            nel.IsEmpty = true;
            nel.Target = _finishState.Single();
            _adjList[curtail].Add(nel);
        }

        private void InstantiateFromMatchFactorOp(MatchFactorWithOp mop)
        {
            var mf = new NFAGraph(mop.Factor);
            CopyIntoWithTerminals(mf);
            switch (mop.Op.Type)
            {
                case UnaryOperatorType.None:
                    break;
                case UnaryOperatorType.Optional:
                    var elink1 = new NFALink();
                    elink1.IsEmpty = true;
                    elink1.Target = _finishState.Single();
                    _adjList[_startState].Add(elink1);
                    break;
                case UnaryOperatorType.OneMany:
                    var elink2 = new NFALink();
                    elink2.IsEmpty = true;
                    elink2.Target = _startState;
                    _adjList[_finishState.Single()].Add(elink2);
                    break;
                case UnaryOperatorType.NoneMany:
                    var newend = NewState();
                    var blink = new NFALink();
                    var flink = new NFALink();
                    blink.IsEmpty = true;
                    flink.IsEmpty = true;
                    blink.Target = _startState;
                    flink.Target = newend;
                    _adjList[_finishState.Single()].Add(blink);
                    _adjList[_startState].Add(flink);
                    _finishState = new HashSet<NFAState>{newend};
                    break;
            }
        }

        private void InstantiateFromMatchFactor(MatchFactor m)
        {
            if (m is SingleChar)
                InstantiateFromSingleChar((SingleChar)m);
            else if (m is CharacterClass)
                InstantiateFromCharClass((CharacterClass)m);
            else if (m is Group)
                InstantiateFromRegex(((Group)m).InnerRegExpr);
        }

        private void InstantiateFromSingleChar(SingleChar C)
        {
            var CClass = new CharacterClass();
            CClass.Elements.Add(new CharacterClassElement(C.Character));
            InstantiateFromCharClass(CClass);
        }

        private void InstantiateFromCharClass(CharacterClass CClass)
        {
            _startState = NewState();
            _finishState.Add(NewState());
            var link = new NFALink();
            link.CharClass = CClass;
            link.Target = _finishState.Single();
            _adjList[_startState].Add(link);
        }

        private NFAState NewState()
        {
            var state = new NFAState();
            _nodes.Add(state);
            _adjList.Add(state, new List<NFALink>());
            return state;
        }

        private void CopyIntoWithTerminals(NFAGraph other)
        {
            CopyInto(other);
            this._startState = other._startState;
            this._finishState = other._finishState;
        }

        private void CopyInto(NFAGraph other)
        {
            foreach (var node in other._nodes)
                this._nodes.Add(node);
            foreach (var kvp in other._adjList)
                this._adjList[kvp.Key] = kvp.Value;
        }

        public bool SimulateMatch(string input)
        {
            return SimulateMatch(input, 0, _startState);
        }

        private bool SimulateMatch(string input, int ptr, NFAState curNode)
        {

            var c = ptr < input.Length ? input[ptr] : (char?)null;
            foreach (var link in _adjList[curNode])
            {
                bool res = false;
                if (c.HasValue && CanFollowLink(link, c.Value))
                    res = SimulateMatch(input, ptr + 1, link.Target);
                else if (link.IsEmpty)
                    res = SimulateMatch(input, ptr, link.Target);

                if (res == true) return true;
            }

            return _finishState.Contains(curNode);
        }

        private bool CanFollowLink(NFALink link, char c)
        {
            if (link.CharClass == null) return false;
            return link.CharClass.Elements.Any(el => el.StartC >= c && el.EndC <= c);
        }

        public NFAGraph ToDFA()
        {
            //first, we need to know the alphabet
            var alphabet = new SortedSet<char>();
            foreach (var state in _adjList.Keys)
            {
                foreach (var link in _adjList[state].Where(l => !l.IsEmpty))
                {
                    alphabet.UnionWith(link.CharClass.CharSet());
                }
            }

            var dfa = new NFAGraph();
            var stateSetMap = new Dictionary<HashSet<NFAState>, NFAState>(HashSet<NFAState>.CreateSetComparer());
            var startStateSet = eClosure(_startState);
            dfa._startState = dfa.NewState();
            stateSetMap[startStateSet] = dfa._startState;

            //Follow the links of the start state
            var markedDFAStates = new HashSet<HashSet<NFAState>>(HashSet<NFAState>.CreateSetComparer());
            //Start at the start and continue whilst there are no unmarked states
            var DFAStateQueue = new Queue<HashSet<NFAState>>();
            DFAStateQueue.Enqueue(startStateSet);
            while (DFAStateQueue.Count > 0)
            {
                var thisstate = DFAStateQueue.Dequeue();
                var origdfastate = stateSetMap[thisstate];
                markedDFAStates.Add(thisstate);
                foreach (var c in alphabet)
                {
                    var cclass = new CharacterClass();
                    cclass.Elements.Add(new CharacterClassElement(c));
                    cclass.IsNegated = false;

                    var targetStates = new HashSet<NFAState>();
                    foreach (var nfastate in thisstate)
                    {
                        foreach (var link in _adjList[nfastate])
                        {
                            if (CanFollowLink(link, c))
                            {
                                targetStates.Add(link.Target);
                            }
                        }
                    }
                    var targetStateClosure = eClosure(targetStates);
                    NFAState ndfastate;
                    if (!markedDFAStates.Contains(targetStateClosure))
                    {
                        DFAStateQueue.Enqueue(targetStateClosure);

                        ndfastate = dfa.NewState();
                        stateSetMap[targetStateClosure] = ndfastate;
                        if (targetStateClosure.Any(s => _finishState.Contains(s)))
                            dfa._finishState.Add(ndfastate);
                    }
                    else
                    {
                        ndfastate = stateSetMap[targetStateClosure];
                    }

                    dfa._adjList[origdfastate].Add(new NFALink()
                    {
                        IsEmpty = false,
                        CharClass = cclass,
                        Target = ndfastate
                    });
                        
                }
                //now check for . transitions
            }

            return dfa;
        }

        private HashSet<NFAState> eClosure(ISet<NFAState> target)
        {
            var rset = new HashSet<NFAState>();
            foreach (var state in target)
                rset.UnionWith(eClosure(state));
            return rset;
        }

        private HashSet<NFAState> eClosure(NFAState target)
        {
            var rset = new HashSet<NFAState>();
            rset.Add(target);
            foreach (var link in _adjList[target])
            {
                if (link.IsEmpty)
                    rset.UnionWith(eClosure(link.Target));
            }
            return rset;
        }

        private HashSet<NFAState> move(ISet<NFAState> source, char c)
        {
            var moveSet = new HashSet<NFAState>();
            foreach (var state in source)
            {
                foreach (var link in _adjList[state])
                {
                    if (CanFollowLink(link, c))
                        moveSet.Add(link.Target);
                }
            }
            return moveSet;
        }
    }
}
