using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theseus.Parser;
using Sprache;
using System.Collections.Generic;
using Theseus.Elements;
using Theseus.Elements.Enumerations;

namespace TheseusTest
{
    [TestClass]
    public class ActionTest : TestBase
    {
        [TestMethod]
        public void ActionParseAdd()
        {
            var actual = TheseusParser.ActionParser.Parse("[[add a to b]]");
            var expected = new Action(new Effect(EffectType.Add, "a", "b"));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ActionParseSet()
        {
            var actual = TheseusParser.ActionParser.Parse("[[set a]]");
            var expected = new Action(new Effect(EffectType.Set, "a", ""));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ActionParseClear()
        {
            var actual = TheseusParser.ActionParser.Parse("[[clear a]]");
            var expected = new Action(new Effect(EffectType.Clear, "a", ""));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ActionParseShow()
        {
            var actual = TheseusParser.ActionParser.Parse("[[show a]]");
            var expected = new Action(new Effect(EffectType.Show, "a"));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ActionParseHide()
        {
            var actual = TheseusParser.ActionParser.Parse("[[hide a]]");
            var expected = new Action(new Effect(EffectType.Hide, "a"));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ActionParseMoveTo()
        {
            var actual = TheseusParser.ActionParser.Parse("[[move to a]]");
            var expected = new Action(new Effect(EffectType.MoveTo, "a"));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ActionParseMultipleEffects()
        {
            var actual = TheseusParser.ActionParser.Parse("[[hide a,show b,add c to d]]");
            var expected = new Action(new List<Effect>
            {
                new Effect(EffectType.Hide, "a"),
                new Effect(EffectType.Show, "b"),
                new Effect(EffectType.Add, "c", "d"),
            });
            AssertEqual(expected, actual);
        }
    }
}

