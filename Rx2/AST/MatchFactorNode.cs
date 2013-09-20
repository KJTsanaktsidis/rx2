using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rx2.CharClassSupport;

namespace Rx2.AST
{
    public enum UnaryOperatorType
    {
        OneMany,
        NoneMany,
        Optional,
        None
    }

    public abstract class MatchFactorNode
    {
        
        public UnaryOperatorType OpType { get; set; }
        public MatchFactorNode Next { get; set; }

        protected MatchFactorNode(UnaryOperatorType op = UnaryOperatorType.None, MatchFactorNode next = null)
        {
            OpType = op;
            Next = next;
        }
    }
}
