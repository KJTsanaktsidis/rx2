using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rx2.AST;

namespace Rx2.CharClassSupport
{
    public static class CharacterClassMapper
    {
        public struct NormaliseCharacterClassResult
        {
            public ISet<CharacterClassElement> Alphabet;
            public IDictionary<CharacterClass, CharacterClass> Mapping;
        }

        private enum TokenType
        {
            Start,
            End
        }

        private struct CharacterRangeToken
        {
            public char Value;
            public CharacterClassElement SourceElement;
            public TokenType Type;

            public override string ToString()
            {
                return "(" + Value + ", " + Type.ToString() + ") from " + SourceElement.ToString();
            }
        }

        public static NormaliseCharacterClassResult NormaliseCharacterClasses(IList<CharacterClass> classes)
        {
            var rStruct = new NormaliseCharacterClassResult()
            {
                Alphabet = new HashSet<CharacterClassElement>(),
                Mapping = new Dictionary<CharacterClass, CharacterClass>()
            };
            
            //Need to create the list of start/end class elements
            var rangeTokenList = classes.SelectMany(cl => cl.Elements)
                .SelectMany(el => new CharacterRangeToken[]
                {
                    new CharacterRangeToken()
                    {
                        Value  = el.Start,
                        SourceElement = el,
                        Type = TokenType.Start
                    },
                    new CharacterRangeToken()
                    {
                        Value = el.End,
                        SourceElement = el,
                        Type = TokenType.End
                    }
                })
                .OrderBy(tk => tk.Value) //stable sort, start tokens always before end tokens
                .ToList();

            //Now break up the ranges
            //Mapping of old char class els into new components
            var activeRanges = new Dictionary<CharacterClassElement, ISet<CharacterClassElement>>();
            var inactiveRanges = new Dictionary<CharacterClassElement, ISet<CharacterClassElement>>();


            var previousToken = new CharacterRangeToken()
            {
                Value = char.MinValue,
                Type = TokenType.Start
            };
            var first = true;

            foreach (var token in rangeTokenList)
            {
                //make our new class
                var newRange = new CharacterClassElement(previousToken.Value, token.Value);
                //work out [), (], etc
                if (previousToken.Type == TokenType.Start && token.Type == TokenType.Start)
                {
                    newRange = new CharacterClassElement(previousToken.Value, (char)(token.Value - 1));
                }
                else if (previousToken.Type == TokenType.End && token.Type == TokenType.End)
                {
                    newRange = new CharacterClassElement((char)(previousToken.Value + 1), token.Value);
                }
                else if (previousToken.Type == TokenType.End && token.Type == TokenType.Start)
                {
                    newRange = new CharacterClassElement((char)(previousToken.Value + 1), (char)(token.Value - 1));
                }

                //Make sure there's actually a worthwile range here
                //Also, skip first so we don't put the \0-token range in
                //If they have an any char class, this will get added by the logic anyway
                if (newRange.End >= newRange.Start && !first)
                {
                    rStruct.Alphabet.Add(newRange);

                    //Now attach it to anything that wants it
                    //Skipped on first iter
                    foreach (var ccel in activeRanges.Keys)
                        activeRanges[ccel].Add(newRange);
                }

                //Now add new source classes
                switch (token.Type)
                {
                    case TokenType.Start:
                        if (!activeRanges.ContainsKey(token.SourceElement))
                            activeRanges.Add(token.SourceElement, new HashSet<CharacterClassElement>());
                        break;
                    case TokenType.End:
                        if (activeRanges.ContainsKey(token.SourceElement))
                        {
                            inactiveRanges[token.SourceElement] = activeRanges[token.SourceElement];
                            activeRanges.Remove(token.SourceElement);
                        }
                        break;
                }

                previousToken = token;
                first = false;
            }

            //fix cclass mappings
            foreach (var cClass in classes)
            {
                rStruct.Mapping[cClass] = new CharacterClass()
                {
                    IsNegated = false
                };
                foreach (var newRange in cClass.Elements.SelectMany(el => inactiveRanges[el])) 
                    rStruct.Mapping[cClass].Elements.Add(newRange);

                //If negated, swap to a non-negated class
                if (cClass.IsNegated)
                {
                    var newCClass = new CharacterClass();
                    newCClass.Elements.UnionWith(
                        rStruct.Alphabet.Except(rStruct.Mapping[cClass].Elements));
                    rStruct.Mapping[cClass] = newCClass;
                }
            }

            return rStruct;
        }

        public static ISet<CharacterClassElement> NormaliseAST(RegexNode rootNode)
        {
            //walk the tree and find all char classes
            var matchClassLink = new Dictionary<CharacterClass, CharacterClassMatchNode>();

            Action<RegexNode> classFinder = null;
            classFinder = (rxNode) =>
            {
                var curAltNode = rxNode.FirstAlternative;
                

                while (curAltNode != null)
                {
                    var curMatchNode = curAltNode.FirstFactor;

                    while (curMatchNode != null)
                    {
                        if (curMatchNode is GroupMatchNode)
                        {
                            var g = (GroupMatchNode) curMatchNode;
                            classFinder(g.Body);
                        }
                        else if (curMatchNode is CharacterClassMatchNode)
                        {
                            var cn = (CharacterClassMatchNode) curMatchNode;
                            matchClassLink[cn.MatchingCharacterClass] = cn;
                        }

                        curMatchNode = curMatchNode.Next;
                    }

                    curAltNode = curAltNode.Next;
                }
            };

            classFinder(rootNode);

            //that's filled, now map them
            //but hide a [min-max] class to make sre the alphabet is total
            var allClass = new CharacterClass();
            allClass.Elements.Add(new CharacterClassElement(char.MinValue, char.MaxValue));
            //matchClassLink[allClass] = null;
            var rStruct = NormaliseCharacterClasses(matchClassLink.Keys.ToList());
            foreach (var cClass in matchClassLink.Keys)
            {
                //if (cClass != allClass) //skip the one we added
                    matchClassLink[cClass].MatchingCharacterClass = rStruct.Mapping[cClass];
            }
            return rStruct.Alphabet;
        }
    }
}
