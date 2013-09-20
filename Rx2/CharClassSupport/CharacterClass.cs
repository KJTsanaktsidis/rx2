using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.CharClassSupport
{
    public struct CharacterClassElement
    {
        public readonly char Start;
        public readonly char End;
        public readonly bool IsEmpty;

        public CharacterClassElement(char start, char end)
        {
            Start = start;
            End = end;
            IsEmpty = false;
        }

        public CharacterClassElement(bool isEmpty)
        {
            IsEmpty = isEmpty;
            Start = '\0';
            End = '\0';
        }

        public override bool Equals(object obj)
        {
            var other = (CharacterClassElement) obj;
            return Start == other.Start && End == other.End;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode()/2 + End.GetHashCode()/2;
        }

        public override string ToString()
        {
            return Start == End
                ? Start.ToString(CultureInfo.InvariantCulture)
                : Start + "-" + End;
        }

        public bool Contains(char c)
        {
            return Start <= c && End >= c;
        }

        public bool Contains(char? c)
        {
            return c.HasValue && Contains(c.Value);
        }
    }

    public class CharacterClass
    {
        public bool IsNegated { get; set; }
        public ISet<CharacterClassElement> Elements { get; private set; } 

        public CharacterClass()
        {
            Elements = new HashSet<CharacterClassElement>();   
        }

        public CharacterClass(IEnumerable<CharacterClassElement> elements)
        {
            Elements = new HashSet<CharacterClassElement>(elements);
        }

        public bool Contains(char c)
        {
            bool response = Elements.Any(el => el.Start <= c && el.End >= c);
            return IsNegated ^ response;
        }

        public bool Contains(char? c)
        {
            return c.HasValue && Contains(c.Value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            if (IsNegated)
                sb.Append("^");
            foreach (var characterClassElement in Elements)
            {
                sb.Append(characterClassElement.ToString());
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
