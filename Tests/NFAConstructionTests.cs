using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rx2.Automaton;
using Rx2.CharClassSupport;
using Rx2.Lexer;
using Rx2.Parser;

namespace Tests
{
    [TestFixture]
    public class NFAConstructionTests
    {
        private bool DoMatch(string re, string input)
        {
            var tokenStream = new TokenStream(new StringReader(re));
            var parser = new RDParser(tokenStream);
            var ast = parser.Parse();
            var nfa = new NFAGraph(ast);

            return nfa.RecursiveMatch(input);
        }

        [Test]
        public void Match1()
        {
            Assert.IsTrue(DoMatch("xxui", "xxui"));
        }

        [Test]
        public void Match2()
        {
            Assert.IsFalse(DoMatch("xxvi", "xxvismeisw"));
        }

        [Test]
        public void Match3()
        {
            Assert.IsTrue(DoMatch("qui|vim", "vim"));
        }

        [Test]
        public void Match4()
        {
            Assert.IsFalse(DoMatch("qui|vim", "nope"));
        }

        [Test]
        public void Match5()
        {
            Assert.IsTrue(DoMatch("ae[iou]+", "aeui"));
        }

        [Test]
        public void Match6()
        {
            Assert.IsFalse(DoMatch("ae[iou]+", "ae"));
        }

        [Test]
        public void Match7()
        {
            Assert.IsTrue(DoMatch("ab*a", "abbbba"));
            Assert.IsTrue(DoMatch("ab*a", "aa"));
        }

        [Test]
        public void Match8()
        {
            Assert.IsTrue(DoMatch("a(b|c)*d", "acbcbcd"));
        }

        [Test]
        public void Match9()
        {
            Assert.IsTrue(DoMatch("(a|b)[^z]+", "by"));
            Assert.IsTrue(DoMatch("(a|b)[^z]+", "byy"));
            Assert.IsFalse(DoMatch("(a|b)[^z]+", "bz"));
            Assert.IsFalse(DoMatch("(a|b)[^z]+", "b"));
        }

        [Test]
        public void Match10()
        {
            Assert.IsTrue(DoMatch("[a-z]+", "lkjalskje"));
            Assert.IsFalse(DoMatch("[a-z]+", "lkjals5kje"));
            Assert.IsTrue(DoMatch("[a-z5]+", "lkjals5kje"));
        }

        [Test]
        public void Match11()
        {
            Assert.IsTrue(DoMatch("a.*a", "amoaie+-`eoeoa"));
        }
    }
}
