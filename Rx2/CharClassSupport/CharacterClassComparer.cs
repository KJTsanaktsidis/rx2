using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.CharClassSupport
{
    public class CharacterClassComparer : IComparer<CharacterClassElement>
    {
        public int Compare(CharacterClassElement x, CharacterClassElement y)
        {
            if (x.Start < y.Start && x.End < y.Start)
                return -1;
            if (y.Start < x.Start && y.End < x.Start)
                return 1;
            if (x.Start == y.Start && x.End == y.End)
                return 0;
            throw new InvalidOperationException("The two ranges are overlapping");
        }
    }
}
