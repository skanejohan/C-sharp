using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using Theseus.Elements;
using Theseus.Elements.Enumerations;
using Theseus.Parser;

namespace TheseusTest
{
    [TestClass]
    public class ItemOptionTests : TestBase
    {
        [TestMethod]
        public void ItemOptionParseOpenable()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("openable");
            var expected = new ItemOption(ItemOptionType.Openable);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParseLockable()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("lockable");
            var expected = new ItemOption(ItemOptionType.Lockable);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParseClosed()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("closed");
            var expected = new ItemOption(ItemOptionType.Closed);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParseLocked()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("locked");
            var expected = new ItemOption(ItemOptionType.Locked);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParseUnlocked()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("unlocked");
            var expected = new ItemOption(ItemOptionType.Unlocked);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParseRequiresCombination()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("requires combination 1964");
            var expected = new ItemOption(ItemOptionType.RequiresCombination, "1964");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParsePickable()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("pickable");
            var expected = new ItemOption(ItemOptionType.Pickable);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParseOpensWhenPicked()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("opensWhenPicked");
            var expected = new ItemOption(ItemOptionType.OpensWhenPicked);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParseRequiresKey()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("requires key goldenKey");
            var expected = new ItemOption(ItemOptionType.RequiresKey, "goldenKey");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemOptionParseContainer()
        {
            var actual = TheseusParser.ItemOptionParser.Parse("container");
            var expected = new ItemOption(ItemOptionType.Container);
            AssertEqual(expected, actual);
        }
    }
}
