using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regex.Parser
{
    public class ParseException : Exception
    {
        public ParseException() : base()
        {
            
        }

        public ParseException(string msg) : base(msg)
        {
            
        }
    }
}
