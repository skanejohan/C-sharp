using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theseus.Parser;
using Sprache;
using System.Collections.Generic;
using Theseus.Elements;
using Theseus.Elements.Enumerations;

namespace TheseusTest
{
    [TestClass]
    public class ItemTests : TestBase
    {
        [TestMethod]
        public void PlusParseEmpty()
        {
            var actual = TheseusParser.PlusParser.Parse("");
            Assert.AreEqual(actual, 0);
        }

        [TestMethod]
        public void PlusParse1()
        {
            var actual = TheseusParser.PlusParser.Parse("+");
            Assert.AreEqual(actual, 1);
        }

        [TestMethod]
        public void PlusParse5()
        {
            var actual = TheseusParser.PlusParser.Parse("+++++");
            Assert.AreEqual(actual, 5);
            actual = TheseusParser.PlusParser.Parse("+++++ ");
            Assert.AreEqual(actual, 5);
            actual = TheseusParser.PlusParser.Parse("+++++A");
            Assert.AreEqual(actual, 5);
        }

        [TestMethod]
        public void PlusParse5Space()
        {
            var actual = TheseusParser.PlusParser.Parse("+++++ ");
            Assert.AreEqual(actual, 5);
        }

        [TestMethod]
        public void PlusParse5A()
        {
            var actual = TheseusParser.PlusParser.Parse("+++++A");
            Assert.AreEqual(actual, 5);
        }

        [TestMethod]
        public void Item1ParsesOK()
        {
            var actual = TheseusParser.ItemParser.Parse("+ item metalBox \"metal box\" container pickable opensWhenPicked requires key paperClip\n" +
                "-------------------\nMost of the color...\n	afterPick=Using the paper clip...");
            var expected = new Item(1, "metalBox", "metal box", false,
                new List<ItemOption>
                {
                    new ItemOption(ItemOptionType.Container),
                    new ItemOption(ItemOptionType.Pickable),
                    new ItemOption(ItemOptionType.OpensWhenPicked),
                    new ItemOption(ItemOptionType.RequiresKey, "paperClip")
                },
                new Section(new SectionText("Most of the color...")),
                new List<Function>(),
                new List<ItemAction>
                {
                    new ItemAction(ItemActionType.AfterPick, new Section(new SectionText("Using the paper clip...")))
                });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void Item2ParsesOK()
        {
            var actual = TheseusParser.ItemParser.Parse(
                "  item frontDoor \"front door\"\n" +
                "  ---------------------------\n" +
                "  This is the main entrance to your book shop...\n" +
                "\n" +
                "  function open \"Open\"\n" +
                "  --------------------\n" +
                "  As you move toward the door...");
            var expected = new Item(0, "frontDoor", "front door", false,
                new List<ItemOption>(),
                new Section(new SectionText("This is the main entrance to your book shop...")),
                new List<Function>
                {
                    new Function("open", "Open", false, new Section(new SectionText("As you move toward the door...")))
                },
                new List<ItemAction>());
            AssertEqual(expected, actual);
        }
    }
}
