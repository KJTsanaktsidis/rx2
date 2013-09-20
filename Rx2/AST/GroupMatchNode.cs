using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx2.AST
{
    public class GroupMatchNode : MatchFactorNode
    {
        public RegexNode Body { get; set; } //Note that start/end anchor props are ignored
        public bool Capturing { get; set; }

        public GroupMatchNode(RegexNode body = null, bool capturing = true)
        {
            Body = body;
            Capturing = capturing;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            if (!Capturing)
                sb.Append("?:");
            sb.Append(Body.ToString());
            sb.Append(")");
            if (Next != null)
                sb.Append(Next.ToString());
            return sb.ToString();
        }
    }
}
