using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System.Collections.Generic;
using Theseus.Elements;
using Theseus.Elements.Enumerations;
using Theseus.Parser;

namespace TheseusTest
{
    [TestClass]
    public class LocationTests : TestBase
    {
        [TestMethod]
        public void LocationParsesOK()
        {
            var actual = TheseusParser.LocationParser.Parse(
                @"location office ""My office""
                  This small office is dominated by a large desk in its centre.");
            var expected = new Location("office", "My office",
                new Section(new SectionText("This small office is dominated by a large desk in its centre.")),
                new List<Flag>(),
                new List<Item>(),
                new List<Door>(),
                new List<Exit>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void LocationWithDashesParsesOK()
        {
            var actual = TheseusParser.LocationParser.Parse(
                @"location office ""My office""
                  -----------------------------
                  This small office is dominated by a large desk in its centre.");
            var expected = new Location("office", "My office",
                new Section(new SectionText("This small office is dominated by a large desk in its centre.")),
                new List<Flag>(),
                new List<Item>(),
                new List<Door>(),
                new List<Exit>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void LocationWithFlagsParsesOK()
        {
            var actual = TheseusParser.LocationParser.Parse(
                @"location office ""My office""
                  -----------------------------
                  This small office is dominated by a large desk in its centre.

                  flag BookshelfPulled is set
                  flag B is not set");
            var expected = new Location("office", "My office",
                new Section(new SectionText("This small office is dominated by a large desk in its centre.")),
                new List<Flag> { new Flag("BookshelfPulled", true), new Flag("B", false) },
                new List<Item>(),
                new List<Door>(),
                new List<Exit>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void LocationWithItemsParsesOK()
        {
            var actual = TheseusParser.LocationParser.Parse(
                @"location office ""My office""
                  -----------------------------
                  This small office is dominated by a large desk in its centre.

                  item historyBookShelf ""history bookshelf""
                  -----------------------------------------
                  The dusty bookshelf...");
            var expected = new Location("office", "My office",
                new Section(new SectionText("This small office is dominated by a large desk in its centre.")),
                new List<Flag> (),
                new List<Item> {
                    new Item(0, "historyBookShelf", "history bookshelf", false,
                        new List<ItemOption>(), new Section(new SectionText("The dusty bookshelf...")), 
                        new List<Function>(), new List<ItemAction>())},
                new List<Door>(),
                new List<Exit>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void LocationWithDoorsParsesOK()
        {
            var actual = TheseusParser.LocationParser.Parse(
                @"location office ""My office""
                  -----------------------------
                  This small office is dominated by a large desk in its centre.

                  door officeDoor ""office door"" lockable locked requires key officeDoorKey");
            var expected = new Location("office", "My office",
                new Section(new SectionText("This small office is dominated by a large desk in its centre.")),
                new List<Flag>(),
                new List<Item>(),
                new List<Door> {
                    new Door("officeDoor", "office door",
                        new List<ItemOption>
                        {
                            new ItemOption(ItemOptionType.Lockable),
                            new ItemOption(ItemOptionType.Locked),
                            new ItemOption(ItemOptionType.RequiresKey, "officeDoorKey"),
                        }
                    ) },
                new List<Exit>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void LocationWithExitsParsesOK()
        {
            var actual = TheseusParser.LocationParser.Parse(
                @"location office ""My office""
                  -----------------------------
                  This small office is dominated by a large desk in its centre.

                  exit south to office via officeDoor
                  exit north to artSection");
            var expected = new Location("office", "My office",
                new Section(new SectionText("This small office is dominated by a large desk in its centre.")),
                new List<Flag>(),
                new List<Item>(),
                new List<Door>(),
                new List<Exit>
                {
                    new Exit(Direction.South, "office", "officeDoor"),
                    new Exit(Direction.North, "artSection")
                });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void LocationWithFlagsItemsDoorsExitsParsesOK()
        {
            var actual = TheseusParser.LocationParser.Parse(
                @"location office ""My office""
                  -----------------------------
                  This small office is dominated by a large desk in its centre.

                  flag A is set
                  flag B is not set

                  item historyBookShelf ""history bookshelf""
                  -----------------------------------------
                  The dusty bookshelf...

                  + item book ""history book""
                  ----------------------------
                  This old book...

                  door officeDoor ""office door"" lockable locked requires key officeDoorKey

                  exit south to office via officeDoor
                  exit north to artSection");
            var expected = new Location("office", "My office",
                new Section(new SectionText("This small office is dominated by a large desk in its centre.")),
                new List<Flag>
                {
                    new Flag("A", true), new Flag("B", false)
                },
                new List<Item>
                {
                    new Item(0, "historyBookShelf", "history bookshelf", false,
                        new List<ItemOption>(), new Section(new SectionText("The dusty bookshelf...")),
                        new List<Function>(), new List<ItemAction>()),
                    new Item(1, "book", "history book", false,
                        new List<ItemOption>(), new Section(new SectionText("This old book...")),
                        new List<Function>(), new List<ItemAction>()),
                },
                new List<Door>
                {
                    new Door("officeDoor", "office door",
                        new List<ItemOption>
                        {
                            new ItemOption(ItemOptionType.Lockable),
                            new ItemOption(ItemOptionType.Locked),
                            new ItemOption(ItemOptionType.RequiresKey, "officeDoorKey"),
                        }
                    )
                },
                new List<Exit>
                {
                    new Exit(Direction.South, "office", "officeDoor"),
                    new Exit(Direction.North, "artSection")
                });
            AssertEqual(expected, actual);
        }
    }
}
