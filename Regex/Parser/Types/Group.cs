using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser.Types
{
    public class Group : MatchFactor
    {
        public RegExpr InnerRegExpr { get; set; }

        public Group(RegExpr innerRegExpr)
        {
            InnerRegExpr = innerRegExpr;
        }
    }
}
