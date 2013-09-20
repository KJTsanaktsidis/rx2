using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.AST
{
    public class RegexNode
    {
        public bool StartAnchor { get; set; }
        public bool EndAnchor { get; set; }
        public AlternativeNode FirstAlternative { get; set; }

        public RegexNode(AlternativeNode firstAlternative = null, bool startAnchor = false, bool endAnchor = false)
        {
            StartAnchor = startAnchor;
            EndAnchor = endAnchor;
            FirstAlternative = firstAlternative;
        }

        public override string ToString()
        {
            return FirstAlternative.ToString();
        }
    }
}
