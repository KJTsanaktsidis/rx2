using System;

namespace Rx2.Parser
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
