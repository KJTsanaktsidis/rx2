using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rx2.Automaton;
using Rx2.Lexer;
using Rx2.Parser;

namespace Tests
{
    [TestFixture]
    public class DFASubmatchTests
    {
        private IList<string> DoSubmatch(string re, string input)
        {
            var tokenStream = new TokenStream(new StringReader(re));
            var parser = new RDParser(tokenStream);
            var ast = parser.Parse();
            var nfa = new NFAGraph(ast);
            var dfa = new DFAGraph(nfa);

            return dfa.SimulateSubmatch(input);
        }

        private bool DoMatch(string re, string input)
        {
            var tokenStream = new TokenStream(new StringReader(re));
            var parser = new RDParser(tokenStream);
            var ast = parser.Parse();
            var nfa = new NFAGraph(ast);
            var dfa = new DFAGraph(nfa);

            return dfa.SimulateMatch(input);
        }

        [Test]
        public void Match1()
        {
            var sm = DoSubmatch("ab(?:c|d)e", "abde");
            Assert.AreEqual(0, sm.Count);
        }

        [Test]
        public void Match2()
        {
            var sm = DoSubmatch("ab(ay|bee)e", "abbeee");
            Assert.AreEqual(1, sm.Count);
            Assert.AreEqual("bee", sm[0]);
        }

        [Test]
        public void Match3()
        {
            var sm = DoSubmatch("ab(ay|bee)*e", "abbeeayaybeee");
            Assert.AreEqual(4, sm.Count);
            CollectionAssert.AreEqual(new String[] {"bee", "ay", "ay", "bee"}, sm);
        }

        [Test]
        public void Match4()
        {
            var sm = DoSubmatch("8(.*)8", "8nine8");
            Assert.AreEqual(1, sm.Count);
            Assert.AreEqual("nine", sm[0]);
        }
    }
}
