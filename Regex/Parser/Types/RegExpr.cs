using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser.Types
{
    public class RegExpr : ParseTreeElement
    {
        private IList<Alternative> _alternatives = new List<Alternative>();
        public IList<Alternative>  Alternatives {get { return _alternatives; }}
    }
}
