using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theseus.Parser;
using Sprache;
using System.Collections.Generic;
using Theseus.Elements;
using Theseus.Elements.Enumerations;

namespace TheseusTest
{
    [TestClass]
    public class DoorTests : TestBase
    {
        [TestMethod]
        public void DoorWithNoOptionsParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\"");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithOpenableParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" openable");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.Openable) });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithOpenParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" open");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.Open) });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithClosedParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" closed");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.Closed) });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithLockableParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" lockable");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.Lockable) });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithLockedParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" locked");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.Locked) });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithUnlockedParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" unlocked");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.Unlocked) });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithRequiresCombinationParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" requires combination 1337");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.RequiresCombination, "1337") });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithPickableParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" pickable");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.Pickable) });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithOpensWhenPickedParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" opensWhenPicked");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.OpensWhenPicked) });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithRequiresKeyParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" requires key mainKey");
            var expected = new Door("mainDoor", "Main door", new List<ItemOption> { new ItemOption(ItemOptionType.RequiresKey, "mainKey") });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void DoorWithMultipleOptionsParsesOK()
        {
            var actual = TheseusParser.DoorParser.Parse("door mainDoor \"Main door\" openable open closed lockable locked unlocked " + 
                "requires combination 1337 pickable opensWhenPicked requires key mainKey");
            var expected = new Door("mainDoor", "Main door", 
                new List<ItemOption>
                {
                    new ItemOption(ItemOptionType.Openable),
                    new ItemOption(ItemOptionType.Open),
                    new ItemOption(ItemOptionType.Closed),
                    new ItemOption(ItemOptionType.Lockable),
                    new ItemOption(ItemOptionType.Locked),
                    new ItemOption(ItemOptionType.Unlocked),
                    new ItemOption(ItemOptionType.RequiresCombination, "1337"),
                    new ItemOption(ItemOptionType.Pickable),
                    new ItemOption(ItemOptionType.OpensWhenPicked),
                    new ItemOption(ItemOptionType.RequiresKey, "mainKey"),
                });
            AssertEqual(expected, actual);
        }

    }
}
