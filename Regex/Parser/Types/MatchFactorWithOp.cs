using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser.Types
{
    public class MatchFactorWithOp : ParseTreeElement
    {
        public MatchFactor Factor { get; private set; }
        public UnaryOperator Op { get; private set; }

        public MatchFactorWithOp(MatchFactor fac, UnaryOperator op)
        {
            Factor = fac;
            Op = op;
        }
    }
}
