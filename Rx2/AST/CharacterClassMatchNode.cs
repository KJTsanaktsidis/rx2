using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rx2.CharClassSupport;

namespace Rx2.AST
{
    public class CharacterClassMatchNode : MatchFactorNode
    {
        public CharacterClass MatchingCharacterClass { get; set; }

        public CharacterClassMatchNode(CharacterClass cClass, UnaryOperatorType op = UnaryOperatorType.None,
            MatchFactorNode next = null) : base(op, next)
        {
            MatchingCharacterClass = cClass;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(MatchingCharacterClass.ToString());
            if (Next != null)
                sb.Append(Next.ToString());
            return sb.ToString();
        }
    }
}
