using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser.Types
{
    public class CharacterClassDecomposition
    {
        public CharacterClass AOnly { get; set; }
        public CharacterClass BOnly { get; set; }
        public CharacterClass Intersection { get; set; }
    }

    public class CharacterClass : MatchFactor
    {
        public static CharacterClass Dot = new CharacterClass(){IsNegated = true};
        public static CharacterClass Empty = new CharacterClass();

        private IList<CharacterClassElement> _elements = new List<CharacterClassElement>(); 
        public IList<CharacterClassElement> Elements {get { return _elements; }}
        public bool IsNegated { get; set; }

        public CharacterClass()
        {
            IsNegated = false;
        }

        public override string ToString()
        {
            return Elements.First().StartC.ToString();
        }


        public ISet<char> CharSet()
        {
            var cset = new SortedSet<char>();
            foreach (var el in _elements)
            {
                for (char i = el.StartC; i <= el.EndC; i++)
                    cset.Add(i);
            }
            return cset;
        }

        /*public void UnionRanges()
        {
            //no foreach, we are modifying
            //but we only delete ahead of the loop ct
            for (int i = 0; i < _elements.Count; i++)
            {
                for (int j = 0; j < _elements.Count; j++)
                {
                    //Are they disjoint?
                    if (_elements[j].StartC <= _elements[i].EndC || _elements[j].EndC >= _elements[i].StartC)
                    {
                        //no.
                        //delete j, rewrite j into i
                        if (_elements[j].StartC < _elements[i].StartC)
                            _elements[i].StartC = _elements[i].EndC;
                        if (_elements[j].EndC > _elements[i].EndC)
                            _elements[i].EndC = _elements[j].EndC;

                        _elements.RemoveAt(j);
                        j--; //make sure we hit the next el
                    }
                }
            }
        }*/


    }
}
