using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rx2.CharClassSupport;

namespace Rx2.CodeGen
{
    public static class DelegateSupport
    {/*
        internal static CharacterClassElement? MapCharacter(char c, ISet<CharacterClassElement> alphabet)
        {
            return alphabet.SingleOrDefault(el => el.Contains(c));
        }
        
        public static CharacterClassElement? MapCharacter(char c, ArraySegment<CharacterClassElement> alphabet)
        {
            //binary search the alphabet for something containing c
            var underlyingArray = alphabet.Array;
            var pivot = alphabet.Count/2 + alphabet.Offset;

            if (alphabet.Count == 0)
            {
                return null;
            }
            if (alphabet.Count == 1 && alphabet[pivot].Contains(c))
            {
                return underlyingArray[]
            }

            if (underlyingArray[pivot].Contains(c))
            {
                return underlyingArray[pivot];
            }
            else if (underlyingArray[pivot].End < c)
            {
                var leftArr = new ArraySegment<CharacterClassElement>(underlyingArray, alphabet.Offset, alphabet.Count/2);
                return MapCharacter(c, leftArr);
            }
            else if (underlyingArray[pivot].Start > c)
            {
                var ct = alphabet.Count - alphabet.Count/2;
                var rightArr = new ArraySegment<CharacterClassElement>(underlyingArray,
                    alphabet.Offset + alphabet.Count/2, ct);
                return MapCharacter(c, rightArr);
            }
        
        }
        */
    }
}
