using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser.Types
{
    public class Alternative : ParseTreeElement
    {
        private IList<MatchFactorWithOp> _factors = new List<MatchFactorWithOp>(); 
        public IList<MatchFactorWithOp> Factors {get { return _factors; }}  
    }
}
