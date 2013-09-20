using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rx2.CharClassSupport;

namespace Rx2.Automaton
{
    public class DFAGraph
    {
        internal readonly IDictionary<DFAState, IDictionary<CharacterClassElement, DFAState>> AdjList;
        internal readonly DFAState StartState;
        internal readonly ISet<DFAState> FinalStates;
        internal readonly ISet<CharacterClassElement> Alphabet; 

        public DFAGraph(NFAGraph nfa)
        {
            Alphabet = nfa.Alphabet;

            FinalStates = new HashSet<DFAState>();
            AdjList = new Dictionary<DFAState, IDictionary<CharacterClassElement, DFAState>>();
            var stateVisitQueue = new Queue<DFAState>();
            var marked = new HashSet<DFAState>();


            //kgo subset construction
            var startEClosure = nfa.EClosure(nfa.StartState);
            StartState = new DFAState(startEClosure);

            stateVisitQueue.Enqueue(StartState);
            if (StartState.Tags.Contains(FAStateTags.Final))
                FinalStates.Add(StartState);
            AdjList[StartState] = new Dictionary<CharacterClassElement, DFAState>();

            while (stateVisitQueue.Count > 0)
            {
                var thisState = stateVisitQueue.Dequeue();
                marked.Add(thisState);

                foreach (var symbol in Alphabet)
                {
                    var u = nfa.EClosure(nfa.Move(thisState.Constituents.ToList(), symbol));
                    if (u.Count > 0)
                    {
                        var dfaU = new DFAState(u);
                        if (!AdjList.Keys.Contains(dfaU))
                        {
                            if (dfaU.Tags.Contains(FAStateTags.Final))
                                FinalStates.Add(dfaU);
                            stateVisitQueue.Enqueue(dfaU);
                            AdjList[dfaU] = new Dictionary<CharacterClassElement, DFAState>();
                        }
                        LinkStates(thisState, dfaU, symbol);
                    }
                    
                }
            }

        }

        private void LinkStates(DFAState src, DFAState dst, CharacterClassElement el)
        {
            AdjList[src][el] = dst;
        }

        public bool SimulateMatch(string input)
        {
            return SimulateMatch(StartState, input);
        }

        private bool SimulateMatch(DFAState node, string input)
        {
            if (input.Length == 0)
                return FinalStates.Contains(node);

            var thisC = input[0];
            var inputRest = input.Substring(1);
            if (!Alphabet.Any(r => r.Contains(thisC)))
                //not in alphabet
                return false;
            var inputSymbol = Alphabet.Single(r => r.Contains(thisC));

            try
            {
                return SimulateMatch(AdjList[node][inputSymbol], inputRest);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public IList<string> SimulateSubmatch(string input)
        {
            var refList = new List<string>();
            SimulateSubmatch(StartState, input, 0, new Stack<int>(), ref refList);
            return refList;
        }

        private void SimulateSubmatch(DFAState node, string input, int pointer, Stack<int> submatchPointers,
            ref List<string> submatches)
        {
            if (pointer == input.Length)
            {
                if (!node.Tags.Contains(FAStateTags.Final))
                    submatches = null;

                return;
            }

            var thisC = input[pointer];
            if (!Alphabet.Any(r => r.Contains(thisC)))
            {
                submatches = null;
                return;
            }

            var inputSymbol = Alphabet.Single(r => r.Contains(thisC));

            //submatch stuff?
            foreach (var tag in node.Tags)
            {
                switch (tag)
                {
                    case FAStateTags.PushSubmatch:
                        submatchPointers.Push(pointer);
                        break;
                    case FAStateTags.PopSubmatch:
                        int mStart = submatchPointers.Pop();
                        submatches.Add(input.Substring(mStart, pointer - mStart));
                        break;
                }
            }

            try
            {
                var nextNode = AdjList[node][inputSymbol];
                SimulateSubmatch(nextNode, input, pointer + 1, submatchPointers, ref submatches);
            }
            catch (KeyNotFoundException)
            {
                submatches = null;
                return;
            }
        }
    }
}
