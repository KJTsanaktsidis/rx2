using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rx2.Lexer;

namespace Tests
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void CanLexBareMatches()
        {
            var lexer = new TokenStream(new StringReader(@"a[bc][^d-ef]"));
            CollectionAssert.AreEqual(new[]
            {
                new CharToken('a'),
                new Token(TokenType.OPENCLASS),
                new CharToken('b'),
                new CharToken('c'),
                new Token(TokenType.CLOSECLASS),
                new Token(TokenType.OPENCLASSNEGATE),
                new CharToken('d'),
                new Token(TokenType.RANGEMARK),
                new CharToken('e'),
                new CharToken('f'),
                new Token(TokenType.CLOSECLASS),
                new Token(TokenType.EOF), 
            }, lexer);
        }

        [Test]
        public void CanLexOperators()
        {
            var lexer = new TokenStream(new StringReader(@"a?.*"));
            CollectionAssert.AreEqual(new[]
            {
                new CharToken('a'),
                new Token(TokenType.OPTIONALOP),
                new Token(TokenType.ANY),
                new Token(TokenType.STAROP),
                new Token(TokenType.EOF), 
            }, lexer);
        }

        [Test]
        public void CanLexGroups()
        {
            var lexer = new TokenStream(new StringReader(@"a(b|c)$"));
            CollectionAssert.AreEqual(new[]
            {
                new CharToken('a'),
                new Token(TokenType.OPENGROUP),
                new CharToken('b'),
                new Token(TokenType.ALTERNATIVE),
                new CharToken('c'),
                new Token(TokenType.CLOSEGROUP),
                new Token(TokenType.ENDANCHOR), 
                new Token(TokenType.EOF), 
            }, lexer);
        }

        [Test]
        public void CanLexEscapes()
        {
            var lexer = new TokenStream(new StringReader(@"^\^*\*"));
            CollectionAssert.AreEqual(new[]
            {
                new Token(TokenType.STARTANCHOR),
                new CharToken('^'),
                new Token(TokenType.STAROP),
                new CharToken('*'),
                new Token(TokenType.EOF), 
            }, lexer);
        }

        [Test]
        public void CanLexInCharClass()
        {
            var lexer = new TokenStream(new StringReader(@"\?[?\]]"));
            CollectionAssert.AreEqual(new[]
            {
                new CharToken('?'),
                new Token(TokenType.OPENCLASS),
                new CharToken('?'),
                new CharToken(']'),
                new Token(TokenType.CLOSECLASS),
                new Token(TokenType.EOF), 
            }, lexer);
        }

        [Test]
        public void CanLexNonCapturing()
        {
            var lexer = new TokenStream(new StringReader(@"(\?a(?:b))"));
            CollectionAssert.AreEqual(new[]
            {
                new Token(TokenType.OPENGROUP),
                new CharToken('?'),
                new CharToken('a'),
                new Token(TokenType.OPENGROUPNOCAP),
                new CharToken('b'),
                new Token(TokenType.CLOSEGROUP),
                new Token(TokenType.CLOSEGROUP),
                new Token(TokenType.EOF), 
            }, lexer);
        }
    }
}
