using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rx2.CharClassSupport;

namespace Tests
{
    [TestFixture]
    public class CharacterClassDecompositionTests
    {
        [Test]
        public void SimpleDecompositionTest()
        {
            var cc1 = new CharacterClass();
            cc1.Elements.Add(new CharacterClassElement('a', 'z'));
            var cc2 = new CharacterClass();
            cc2.Elements.Add(new CharacterClassElement('f', 'q'));
            cc2.Elements.Add(new CharacterClassElement('j', 's'));

            var newMappings = CharacterClassMapper.NormaliseCharacterClasses(new[] {cc1, cc2});
            
            Assert.AreEqual(5, newMappings.Alphabet.Count);
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('a', 'e'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('f', 'i'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('j', 'q'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('r', 's'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('t', 'z'));

            Assert.AreEqual(5, newMappings.Mapping[cc1].Elements.Count);
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('a', 'e'));
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('f', 'i'));
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('j', 'q'));
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('r', 's'));
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('t', 'z'));

            Assert.AreEqual(3, newMappings.Mapping[cc2].Elements.Count);
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('f', 'i'));
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('j', 'q'));
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('r', 's'));

        }

        [Test]
        public void SingleDecompositionTest()
        {
            var cc1 = new CharacterClass();
            cc1.Elements.Add(new CharacterClassElement('a', 'k'));
            cc1.Elements.Add(new CharacterClassElement('v', 'v'));
            var cc2 = new CharacterClass();
            cc2.Elements.Add(new CharacterClassElement('l', 'z'));
            cc2.Elements.Add(new CharacterClassElement('b', 'b'));

            var newMappings = CharacterClassMapper.NormaliseCharacterClasses(new[] {cc1, cc2});

            Assert.AreEqual(6, newMappings.Alphabet.Count);
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('a', 'a'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('b', 'b'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('c', 'k'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('l', 'u'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('v', 'v'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('w', 'z'));
            
            Assert.AreEqual(4, newMappings.Mapping[cc1].Elements.Count);
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('a', 'a'));
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('b', 'b'));
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('c', 'k'));
            CollectionAssert.Contains(newMappings.Mapping[cc1].Elements, new CharacterClassElement('v', 'v'));

            Assert.AreEqual(4, newMappings.Mapping[cc2].Elements.Count);
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('l', 'u'));
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('v', 'v'));
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('w', 'z'));
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('b', 'b'));
        }

        [Test]
        public void DecompositionWithAnyClassTest()
        {
            var cc1 = new CharacterClass();
            cc1.Elements.Add(new CharacterClassElement(char.MinValue, char.MaxValue));
            var cc2 = new CharacterClass();
            cc2.Elements.Add(new CharacterClassElement('b', 'k'));
            cc2.Elements.Add(new CharacterClassElement('w', 'w'));

            var newMappings = CharacterClassMapper.NormaliseCharacterClasses(new[] { cc1, cc2 });

            Assert.AreEqual(5, newMappings.Alphabet.Count);
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement(char.MinValue, 'a'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('b', 'k'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('l', 'v'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('w', 'w'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('x', char.MaxValue));

            Assert.AreEqual(2, newMappings.Mapping[cc2].Elements.Count);
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('b', 'k'));
            CollectionAssert.Contains(newMappings.Mapping[cc2].Elements, new CharacterClassElement('w', 'w'));
            
            CollectionAssert.AreEquivalent(newMappings.Alphabet, newMappings.Mapping[cc1].Elements);
        }

        [Test]
        public void NegatedRangesDecompositionTest()
        {
            var ccn = new CharacterClass();
            ccn.IsNegated = true;
            ccn.Elements.Add(new CharacterClassElement('b', 'q'));
            var ccAll = new CharacterClass();
            ccAll.Elements.Add(new CharacterClassElement(char.MinValue, char.MaxValue));
            var ccMiddle = new CharacterClass();
            ccMiddle.Elements.Add(new CharacterClassElement('k', 'm'));
            var ccEnd = new CharacterClass();
            ccEnd.Elements.Add(new CharacterClassElement('t', 'y'));

            var newMappings = CharacterClassMapper.NormaliseCharacterClasses(new[] {ccn, ccAll, ccMiddle, ccEnd});

            Assert.AreEqual(7, newMappings.Alphabet.Count);
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement(char.MinValue, 'a'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('b', 'j'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('k', 'm'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('n', 'q'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('r', 's'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('t', 'y'));
            CollectionAssert.Contains(newMappings.Alphabet, new CharacterClassElement('z', char.MaxValue));

            CollectionAssert.AreEquivalent(newMappings.Alphabet, newMappings.Mapping[ccAll].Elements);

            Assert.IsFalse(newMappings.Mapping[ccn].IsNegated);
            CollectionAssert.AreEquivalent(new[]
            {
                new CharacterClassElement(char.MinValue, 'a'),
                new CharacterClassElement('r', 's'),
                new CharacterClassElement('t', 'y'),
                new CharacterClassElement('z', char.MaxValue) 
            }, newMappings.Mapping[ccn].Elements);

            CollectionAssert.AreEquivalent(new[]
            {
                new CharacterClassElement('k', 'm')    
            }, newMappings.Mapping[ccMiddle].Elements);
        }

        [Test]
        public void CanDecomposeDupes()
        {
            var cc1 = new CharacterClass();
            cc1.Elements.Add(new CharacterClassElement('b', 'y'));
            var cc2 = new CharacterClass();
            cc2.Elements.Add(new CharacterClassElement('b', 'y'));

            var newMappings = CharacterClassMapper.NormaliseCharacterClasses(new[] {cc1, cc2});

            Assert.AreEqual(1, newMappings.Alphabet.Count);
            Assert.AreEqual(new CharacterClassElement('b', 'y'), newMappings.Alphabet.First());
            Assert.AreEqual(new CharacterClassElement('b', 'y'), newMappings.Mapping[cc1].Elements.First());
            Assert.AreEqual(new CharacterClassElement('b', 'y'), newMappings.Mapping[cc2].Elements.First());
        }

        [Test]
        public void CanDecomposePartialDupes()
        {
            var cc1 = new CharacterClass();
            cc1.Elements.Add(new CharacterClassElement('b', 'y'));
            var cc2 = new CharacterClass();
            cc2.Elements.Add(new CharacterClassElement('b', 'v'));

            var newMappings = CharacterClassMapper.NormaliseCharacterClasses(new[] { cc1, cc2 });

            CollectionAssert.AreEquivalent(new[]
            {
                new CharacterClassElement('b', 'v'),
                new CharacterClassElement('w', 'y') 
            }, newMappings.Alphabet);
            CollectionAssert.AreEquivalent(new[]
            {
                new CharacterClassElement('b', 'v'),
                new CharacterClassElement('w', 'y') 
            }, newMappings.Mapping[cc1].Elements);
            CollectionAssert.AreEquivalent(new[]
            {
                new CharacterClassElement('b', 'v'),
            }, newMappings.Mapping[cc2].Elements);
        }
    }
}
