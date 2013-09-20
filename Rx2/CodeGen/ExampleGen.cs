using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rx2.CharClassSupport;

namespace Rx2.CodeGen
{
    class ExampleGen
    {

        private struct SubMatch
        {
            public int Start;
            public int End;
        }

        /*private bool ExampleDelegate(string input)
        {
            int state = 0;

            var matchStack = new Stack<int>();
            var matches = new List<SubMatch>();
            var alphaSet = new SortedSet<CharacterClassElement>();
            var alphaList = alphaSet.ToList();
            for (int ip = 0; ip < input.Length; ip++)
            {
                var c = input[ip];
                

                switch (state)
                {
                    case 1:
                        switch (alphaEl)
                        {
                            case 
                        }
                    case 2:
                        break;
                    default:
                        return false;
                }
            }
        }*/
    }
}
