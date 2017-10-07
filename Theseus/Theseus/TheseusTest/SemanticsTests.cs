using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theseus.Interfaces;
using Theseus.Semantics;
using Theseus.Elements;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Enumerations;

namespace TheseusTest
{
    [TestClass]
    public class SemanticsTests
    {
        ISemantics semanticsManager;
        IEnumerable<ItemOption> options;
        IEnumerable<Function> functions;
        IEnumerable<ItemAction> actions;

        [TestInitialize]
        public void Initialize()
        {
            semanticsManager = new SemanticsManager();
            options = new List<ItemOption>();
            functions = new List<Function>();
            actions = new List<ItemAction>();
        }

        [TestMethod]
        public void ItemsAddedShouldBeAvailable()
        {
            semanticsManager.AddItem(new Item(0, "item0", "Item 0", false, options, null, functions, actions));
            semanticsManager.AddItem(new Item(1, "item1", "Item 1", false, options, null, functions, actions));
            semanticsManager.AddItem(new Item(2, "item2", "Item 2", false, options, null, functions, actions));
            Assert.AreEqual(3, semanticsManager.Items.Count());
        }

        [TestMethod]
        public void DuplicateItemsShouldNotBeAllowed()
        {
            semanticsManager.AddItem(new Item(0, "item0", "Item 0", false, options, null, functions, actions));
            semanticsManager.AddItem(new Item(1, "item1", "Item 1", false, options, null, functions, actions));
            semanticsManager.AddItem(new Item(2, "item0", "Item 0", false, options, null, functions, actions));
            Assert.AreEqual(2, semanticsManager.Items.Count());
        }

        [TestMethod]
        public void DuplicateItemsShouldGenerateEvent()
        {
            var eventStruck = false;
            semanticsManager.ItemAlreadyExists += (o, e) => eventStruck = true;
            semanticsManager.AddItem(new Item(0, "item0", "Item 0", false, options, null, functions, actions));
            semanticsManager.AddItem(new Item(1, "item1", "Item 1", false, options, null, functions, actions));
            semanticsManager.AddItem(new Item(2, "item0", "Item 0", false, options, null, functions, actions));
            Assert.AreEqual(2, semanticsManager.Items.Count());
            Assert.AreEqual(true, eventStruck);
        }

        [TestMethod]
        public void FlagsAddedShouldBeAvailable()
        {
            semanticsManager.AddFlag(new Flag("flag0", false));
            semanticsManager.AddFlag(new Flag("flag1", false));
            semanticsManager.AddFlag(new Flag("flag2", false));
            Assert.AreEqual(3, semanticsManager.Flags.Count());
        }

        [TestMethod]
        public void DuplicateFlagsShouldNotBeAllowed()
        {
            semanticsManager.AddFlag(new Flag("flag0", false));
            semanticsManager.AddFlag(new Flag("flag1", false));
            semanticsManager.AddFlag(new Flag("flag0", false));
            Assert.AreEqual(2, semanticsManager.Flags.Count());
        }

        [TestMethod]
        public void DuplicateFlagsShouldGenerateEvent()
        {
            var eventStruck = false;
            semanticsManager.FlagAlreadyExists += (o, e) => eventStruck = true;
            semanticsManager.AddFlag(new Flag("flag0", false));
            semanticsManager.AddFlag(new Flag("flag1", false));
            semanticsManager.AddFlag(new Flag("flag0", false));
            Assert.AreEqual(2, semanticsManager.Flags.Count());
            Assert.AreEqual(true, eventStruck);
        }

        [TestMethod]
        public void DoorsAddedShouldBeAvailable()
        {
            semanticsManager.AddDoor(new Door("door0", "Door 0", options));
            semanticsManager.AddDoor(new Door("door1", "Door 1", options));
            semanticsManager.AddDoor(new Door("door2", "Door 2", options));
            Assert.AreEqual(3, semanticsManager.Doors.Count());
        }

        [TestMethod]
        public void DuplicateDoorsShouldNotBeAllowed()
        {
            semanticsManager.AddDoor(new Door("door0", "Door 0", options));
            semanticsManager.AddDoor(new Door("door1", "Door 1", options));
            semanticsManager.AddDoor(new Door("door0", "Door 0", options));
            Assert.AreEqual(2, semanticsManager.Doors.Count());
        }

        [TestMethod]
        public void DuplicateDoorsShouldGenerateEvent()
        {
            var eventStruck = false;
            semanticsManager.DoorAlreadyExists += (o, e) => eventStruck = true;
            semanticsManager.AddDoor(new Door("door0", "Door 0", options));
            semanticsManager.AddDoor(new Door("door1", "Door 1", options));
            semanticsManager.AddDoor(new Door("door0", "Door 0", options));
            Assert.AreEqual(2, semanticsManager.Doors.Count());
            Assert.AreEqual(true, eventStruck);
        }

        [TestMethod]
        public void FunctionsAddedShouldBeAvailable()
        {
            semanticsManager.AddFunction(new Function("func0", "Func 0", false, null));
            semanticsManager.AddFunction(new Function("func1", "Func 1", false, null));
            semanticsManager.AddFunction(new Function("func2", "Func 2", false, null));
            Assert.AreEqual(3, semanticsManager.Functions.Count());
        }

        [TestMethod]
        public void DuplicateFunctionsShouldNotBeAllowed()
        {
            semanticsManager.AddFunction(new Function("func0", "Func 0", false, null));
            semanticsManager.AddFunction(new Function("func1", "Func 1", false, null));
            semanticsManager.AddFunction(new Function("func0", "Func 0", false, null));
            Assert.AreEqual(2, semanticsManager.Functions.Count());
        }

        [TestMethod]
        public void DuplicateFunctionsShouldGenerateEvent()
        {
            var eventStruck = false;
            semanticsManager.FunctionAlreadyExists += (o, e) => eventStruck = true;
            semanticsManager.AddFunction(new Function("func0", "Func 0", false, null));
            semanticsManager.AddFunction(new Function("func1", "Func 1", false, null));
            semanticsManager.AddFunction(new Function("func0", "Func 0", false, null));
            Assert.AreEqual(2, semanticsManager.Functions.Count());
            Assert.AreEqual(true, eventStruck);
        }

        [TestMethod]
        public void ExitsAddedShouldBeAvailable()
        {
            semanticsManager.AddExit(new Exit(Direction.East, "target1"));
            semanticsManager.AddExit(new Exit(Direction.West, "target2"));
            semanticsManager.AddExit(new Exit(Direction.South, "target3"));
            Assert.AreEqual(3, semanticsManager.Exits.Count());
        }

        [TestMethod]
        public void ExitsDifferingOnlyInDirectionShouldBeAvailable()
        {
            semanticsManager.AddExit(new Exit(Direction.East, "target1"));
            semanticsManager.AddExit(new Exit(Direction.West, "target2"));
            semanticsManager.AddExit(new Exit(Direction.South, "target1"));
            Assert.AreEqual(3, semanticsManager.Exits.Count());
        }

        [TestMethod]
        public void ExitsDifferingOnlyInTargetShouldBeAvailable()
        {
            semanticsManager.AddExit(new Exit(Direction.East, "target1"));
            semanticsManager.AddExit(new Exit(Direction.West, "target2"));
            semanticsManager.AddExit(new Exit(Direction.East, "target3"));
            Assert.AreEqual(3, semanticsManager.Exits.Count());
        }

        [TestMethod]
        public void ExitsDifferingOnlyInDoorShouldBeAvailable()
        {
            semanticsManager.AddExit(new Exit(Direction.East, "target1"));
            semanticsManager.AddExit(new Exit(Direction.West, "target2"));
            semanticsManager.AddExit(new Exit(Direction.East, "target1", "door1"));
            Assert.AreEqual(3, semanticsManager.Exits.Count());
        }

        [TestMethod]
        public void DuplicateExitsShouldNotBeAllowed()
        {
            semanticsManager.AddExit(new Exit(Direction.East, "target1"));
            semanticsManager.AddExit(new Exit(Direction.West, "target2"));
            semanticsManager.AddExit(new Exit(Direction.East, "target1"));
            Assert.AreEqual(2, semanticsManager.Exits.Count());
        }

        [TestMethod]
        public void DuplicateExitsShouldGenerateEvent()
        {
            var eventStruck = false;
            semanticsManager.ExitAlreadyExists += (o, e) => eventStruck = true;
            semanticsManager.AddExit(new Exit(Direction.East, "target1"));
            semanticsManager.AddExit(new Exit(Direction.West, "target2"));
            semanticsManager.AddExit(new Exit(Direction.East, "target1"));
            Assert.AreEqual(2, semanticsManager.Exits.Count());
            Assert.AreEqual(true, eventStruck);
        }
    }
}
