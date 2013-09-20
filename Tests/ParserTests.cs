using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rx2.AST;
using Rx2.CharClassSupport;
using Rx2.Lexer;
using Rx2.Parser;

namespace Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void CanParseCharClass()
        {
            var tks = new[]
            {
                new Token(TokenType.OPENCLASSNEGATE),
                new CharToken('A'),
                new Token(TokenType.RANGEMARK),
                new CharToken('Z'),
                new CharToken('q'),
                new Token(TokenType.CLOSECLASS),
                new Token(TokenType.EOF), 
            };

            var parser = new RDParser(tks);
            var rxNode = parser.Parse();

            var resultClass = ((CharacterClassMatchNode) rxNode.FirstAlternative.FirstFactor).MatchingCharacterClass;

            CollectionAssert.Contains(resultClass.Elements, new CharacterClassElement('A', 'Z'));
            CollectionAssert.Contains(resultClass.Elements, new CharacterClassElement('q', 'q'));
            Assert.AreEqual(2, resultClass.Elements.Count);
        }

        [Test]
        public void CanParseChainedMatch()
        {
            var tks = new[]
            {
                new CharToken('e'),
                new CharToken('f'),
                new Token(TokenType.OPENCLASS),
                new CharToken('r'),
                new Token(TokenType.RANGEMARK),
                new CharToken('w'),
                new Token(TokenType.CLOSECLASS),
                new Token(TokenType.OPENCLASSNEGATE),
                new CharToken('a'),
                new CharToken('b'),
                new Token(TokenType.CLOSECLASS),
                new Token(TokenType.EOF)
            };

            var parser = new RDParser(tks);
            var rxNode = parser.Parse();

            var alt = rxNode.FirstAlternative;
            Assert.IsNull(alt.Next);
            var m = alt.FirstFactor;
            Assert.IsInstanceOf<CharacterClassMatchNode>(m);
            var ccm = (CharacterClassMatchNode) m;
            Assert.AreEqual(new CharacterClassElement('e', 'e'), ccm.MatchingCharacterClass.Elements.Single());
            ccm = (CharacterClassMatchNode) ccm.Next;
            Assert.AreEqual(new CharacterClassElement('f', 'f'), ccm.MatchingCharacterClass.Elements.Single());
            ccm = (CharacterClassMatchNode)ccm.Next;
            Assert.AreEqual(new CharacterClassElement('r', 'w'), ccm.MatchingCharacterClass.Elements.Single());
            ccm = (CharacterClassMatchNode)ccm.Next;
            CollectionAssert.Contains(ccm.MatchingCharacterClass.Elements, new CharacterClassElement('a', 'a'));
            CollectionAssert.Contains(ccm.MatchingCharacterClass.Elements, new CharacterClassElement('b', 'b'));
            Assert.IsTrue(ccm.MatchingCharacterClass.IsNegated);
            Assert.IsNull(ccm.Next);
        }

        [Test]
        public void CanParseChainedAlternatives()
        {
            var tks = new[]
            {
                new CharToken('a'),
                new Token(TokenType.ALTERNATIVE),
                new Token(TokenType.OPENCLASS),
                new CharToken('r'),
                new Token(TokenType.RANGEMARK),
                new CharToken('w'),
                new Token(TokenType.CLOSECLASS),
                new Token(TokenType.EOF),
            };

            var parser = new RDParser(tks);
            var rxNode = parser.Parse();

            var alt1 = rxNode.FirstAlternative;
            var alt2 = alt1.Next;
            Assert.IsNull(alt2.Next);

            Assert.IsInstanceOf<CharacterClassMatchNode>(alt1.FirstFactor);
            Assert.IsInstanceOf<CharacterClassMatchNode>(alt2.FirstFactor);
            var ccm1 = (CharacterClassMatchNode) alt1.FirstFactor;
            var ccm2 = (CharacterClassMatchNode) alt2.FirstFactor;
            Assert.AreEqual(new CharacterClassElement('a', 'a'), ccm1.MatchingCharacterClass.Elements.Single());
            Assert.AreEqual(new CharacterClassElement('r', 'w'), ccm2.MatchingCharacterClass.Elements.Single());
        }

        [Test]
        public void CanDoUnaryOps()
        {
            var tks = new[]
            {
                new CharToken('a'),
                new Token(TokenType.OPTIONALOP),
                new Token(TokenType.ANY),
                new Token(TokenType.STAROP),
                new Token(TokenType.EOF),
            };

            var parser = new RDParser(tks);
            var rxNode = parser.Parse();
            
            var alt = rxNode.FirstAlternative;
            var m1 = (CharacterClassMatchNode) alt.FirstFactor;
            var m2 = (CharacterClassMatchNode) m1.Next;

            Assert.IsFalse(rxNode.StartAnchor);
            Assert.IsFalse(rxNode.EndAnchor);

            Assert.AreEqual(UnaryOperatorType.Optional, m1.OpType);
            Assert.AreEqual(UnaryOperatorType.NoneMany, m2.OpType);
            Assert.AreEqual(new CharacterClassElement(char.MinValue, char.MaxValue), m2.MatchingCharacterClass.Elements.First());

        }

        [Test]
        public void CanDoAnchoring()
        {
            var tks = new[]
            {
                new Token(TokenType.STARTANCHOR), 
                new CharToken('a'),
                new Token(TokenType.OPTIONALOP),
                new Token(TokenType.ALTERNATIVE), 
                new Token(TokenType.ANY),
                new Token(TokenType.STAROP),
                new Token(TokenType.ENDANCHOR),
                new Token(TokenType.EOF),
            };

            var parser = new RDParser(tks);
            var rxNode = parser.Parse();

            Assert.IsTrue(rxNode.StartAnchor);
            Assert.IsTrue(rxNode.EndAnchor);
        }

        [Test]
        public void CanParseGroup()
        {
            var tks = new[]
            {
                new Token(TokenType.OPENGROUP),
                new CharToken('a'),
                new Token(TokenType.ALTERNATIVE),
                new CharToken('b'),
                new Token(TokenType.CLOSEGROUP),
                new Token(TokenType.PLUSOP),
                new Token(TokenType.EOF), 
            };

            var parser = new RDParser(tks);
            var rxNode = parser.Parse();

            var alt = rxNode.FirstAlternative;
            Assert.IsNull(alt.Next);
            var group = (GroupMatchNode) alt.FirstFactor;
            Assert.IsNull(group.Next);
            Assert.IsTrue(group.Capturing);
            var innerAlt1 = group.Body.FirstAlternative;
            var innerAlt2 = innerAlt1.Next;
            Assert.IsNull(innerAlt2.Next);
            var m1 = (CharacterClassMatchNode) innerAlt1.FirstFactor;
            var m2 = (CharacterClassMatchNode) innerAlt2.FirstFactor;
            Assert.AreEqual(new CharacterClassElement('a', 'a'), m1.MatchingCharacterClass.Elements.Single());
            Assert.AreEqual(new CharacterClassElement('b', 'b'), m2.MatchingCharacterClass.Elements.Single());
            Assert.AreEqual(UnaryOperatorType.OneMany, group.OpType);
        }
    }
}
