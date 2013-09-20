using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser.Types
{

    public class CharacterClassElement : ParseTreeElement
    {
        public char StartC { get; set; }
        public char EndC { get; set; }

        public CharacterClassElement(char c)
        {
            StartC = c;
            EndC = c;
        }

        public CharacterClassElement(char cL, char cR)
        {
            StartC = cL;
            EndC = cR;
        }
    }
}
