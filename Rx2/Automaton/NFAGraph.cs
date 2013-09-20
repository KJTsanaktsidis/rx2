using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using Rx2.AST;
using Rx2.CharClassSupport;

namespace Rx2.Automaton
{
    public class NFAGraph
    {
        internal readonly IDictionary<NFAState, IDictionary<CharacterClassElement, ISet<NFAState>>> AdjList;
        internal readonly StateFactory StateFac;
        internal readonly NFAState FinalState;
        internal readonly NFAState StartState;
        internal readonly ISet<CharacterClassElement> Alphabet;

        internal static readonly CharacterClassElement Empty = new CharacterClassElement(true);

        public NFAGraph(RegexNode rxNode, StateFactory fac = null)
        {
            Alphabet = CharacterClassMapper.NormaliseAST(rxNode);

            StateFac = fac ?? new StateFactory();
            AdjList = new Dictionary<NFAState, IDictionary<CharacterClassElement, ISet<NFAState>>>();

            //begin from the start
            StartState = MakeState();
            StartState.Tags.Add(FAStateTags.Start);
            FinalState = MakeState();
            FinalState.Tags.Add(FAStateTags.Final);
            //every alternative is connected to this
            for (var altNode = rxNode.FirstAlternative; altNode != null; altNode = altNode.Next)
            {
                //node for this alt
                var altPosNode = MakeState();
                LinkStates(StartState, altPosNode, Empty);
                NFAState oldPosNode = null;

                //Iter the and matches
                for (var matchNode = altNode.FirstFactor; matchNode != null; matchNode = matchNode.Next)
                {
                    oldPosNode = altPosNode;
                    if (matchNode is CharacterClassMatchNode)
                    {
                        var ccn = (CharacterClassMatchNode) matchNode;
                        var thisState = MakeState();
                        LinkStates(altPosNode, thisState, ccn.MatchingCharacterClass);
                        altPosNode = thisState;
                    }
                    else if (matchNode is GroupMatchNode)
                    {
                        var gn = (GroupMatchNode) matchNode;
                        //Create a NFA for the group and join it in
                        var groupGraph = new NFAGraph(gn.Body, StateFac);
                        //Merge all their states into ours
                        foreach (var oState in groupGraph.AdjList.Keys)
                            AdjList[oState] = groupGraph.AdjList[oState];
                        LinkStates(altPosNode, groupGraph.StartState, Empty);

                        //tag it
                        groupGraph.StartState.Tags.Remove(FAStateTags.Start);
                        groupGraph.StartState.Tags.Add(FAStateTags.PushSubmatch);
                        groupGraph.FinalState.Tags.Remove(FAStateTags.Final); //destroys final tag as intended
                        groupGraph.FinalState.Tags.Add(FAStateTags.PopSubmatch); 

                        altPosNode = groupGraph.FinalState;
                    }

                    //operators?
                    switch (matchNode.OpType)
                    {
                        case UnaryOperatorType.NoneMany: //kleene star
                            //link the state we just made back via e
                            LinkStates(altPosNode, oldPosNode, Empty);
                            var newTermination = MakeState();
                            LinkStates(oldPosNode, newTermination, Empty);
                            oldPosNode = altPosNode;
                            altPosNode = newTermination;
                            break;
                        case UnaryOperatorType.Optional:
                            LinkStates(oldPosNode, altPosNode, Empty); //skip path we just made
                            break;
                        case UnaryOperatorType.OneMany:
                            LinkStates(altPosNode, oldPosNode, Empty); //make a cycle
                            break;
                    }

                }

                //join it into the end
                LinkStates(altPosNode, FinalState, Empty);
            }
        }

        private NFAState MakeState()
        {
            var state = StateFac.MakeState();
            AdjList[state] = new Dictionary<CharacterClassElement, ISet<NFAState>>();
            return state;
        }

        private void LinkStates(NFAState src, NFAState dst, CharacterClass cond)
        {
            foreach (var el in cond.Elements)
                LinkStates(src, dst, el);
        }

        private void LinkStates(NFAState src, NFAState dst, CharacterClassElement el)
        {
            if (!AdjList[src].ContainsKey(el) || AdjList[src][el] == null)
                AdjList[src][el] = new HashSet<NFAState>();
            AdjList[src][el].Add(dst);
        }

        public bool RecursiveMatch(string input)
        {
            return RecursiveMatch(StartState, input);
        }

        private bool RecursiveMatch(NFAState node, string input)
        {
            var thisChar = input.Length > 0
                ? input[0]
                : (char?) null;
            var inputRest = input.Length > 0
                ? input.Substring(1)
                : input;

            foreach (var classOption in AdjList[node].Keys)
            {
                bool res = false;
                //can we travel down this link?
                if (classOption.Contains(thisChar))
                {
                    foreach (var target in AdjList[node][classOption])
                    {
                        res = RecursiveMatch(target, inputRest);
                        if (res) break;
                    }
                }
                else if (classOption.IsEmpty)
                {
                    foreach (var target in AdjList[node][classOption])
                    {
                        res = RecursiveMatch(target, input);
                        if (res) break;
                    }
                }

                if (res) return true;
            }


            //base case:
            return node == FinalState && input.Length == 0;
        }

        internal IList<NFAState> EClosure(NFAState closureState)
        {
            var s = new List<NFAState>();
            s.Add(closureState);
            return EClosure(s);
        }

        internal IList<NFAState> EClosure(ISet<NFAState> closureStates)
        {
            return EClosure(closureStates.ToList());
        }

        internal IList<NFAState> EClosure(IList<NFAState> closureStates)
        {
            var rSet = new List<NFAState>();
            var stateStack = new Stack<NFAState>();
            rSet.AddRange(closureStates);
            foreach (var state in rSet)
                stateStack.Push(state);

            while (stateStack.Count > 0)
            {
                var sourceState = stateStack.Pop();
                if (AdjList[sourceState].ContainsKey(Empty))
                {
                    foreach (var visitState in AdjList[sourceState][Empty])
                    {
                        rSet.Add(visitState);
                        stateStack.Push(visitState);
                    }
                }
            }

            return rSet.Distinct().ToList();
        }

        internal IList<NFAState> Move(NFAState moveState, CharacterClassElement input)
        {
            var s = new List<NFAState>();
            s.Add(moveState);
            return Move(s, input);
        }

        internal IList<NFAState> Move(IList<NFAState> moveStates, CharacterClassElement input)
        {
            var rSet = new List<NFAState>();
            foreach (var state in moveStates)
            {
                if (AdjList[state].ContainsKey(input))
                    rSet.AddRange(AdjList[state][input]);
            }

            return rSet.Distinct().ToList();
        }
    }
}
