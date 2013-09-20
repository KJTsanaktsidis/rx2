using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.AST
{
    public class AlternativeNode
    {
        public AlternativeNode Next { get; set; }
        public MatchFactorNode FirstFactor { get; set; }

        public AlternativeNode(MatchFactorNode firstFactor = null, AlternativeNode next = null)
        {
            Next = next;
            FirstFactor = firstFactor;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var curNode = this;
            while (curNode != null)
            {
                if (curNode.FirstFactor != null)
                {
                    sb.Append(curNode.FirstFactor.ToString());
                    sb.Append("|");
                }
                curNode = curNode.Next;
            }
            sb.Remove(sb.Length - 1, 1); //trailing |
            return sb.ToString();
        }
    }
}
