using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser.Types
{
    public enum UnaryOperatorType
    {
        OneMany,
        NoneMany,
        Optional,
        None
    }

    public class UnaryOperator : ParseTreeElement
    {
        public UnaryOperatorType Type { get; private set; }

        public UnaryOperator(UnaryOperatorType T)
        {
            Type = T;
        }
    }
}
