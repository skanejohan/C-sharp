using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using Theseus.Elements;
using Theseus.Elements.Enumerations;
using Theseus.Parser;

namespace TheseusTest
{
    [TestClass]
    public class ItemActionTests : TestBase
    {
        [TestMethod]
        public void ItemActionAfterDropOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterDrop = You drop the item");
            var expected = new ItemAction(ItemActionType.AfterDrop, new Section(new SectionText("You drop the item")));
            AssertEqual(expected, actual);
        }

        public void ItemActionAfterDropOnceOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterDropOnce = You drop the item");
            var expected = new ItemAction(ItemActionType.AfterDropOnce, new Section(new SectionText("You drop the item")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionAfterTakeOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterTake = You take the item");
            var expected = new ItemAction(ItemActionType.AfterTake, new Section(new SectionText("You take the item")));
            AssertEqual(expected, actual);
        }

        public void ItemActionAfterTakeOnceOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterTakeOnce = You take the item");
            var expected = new ItemAction(ItemActionType.AfterTakeOnce, new Section(new SectionText("You take the item")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionAfterCloseOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterClose = You close the item");
            var expected = new ItemAction(ItemActionType.AfterClose, new Section(new SectionText("You close the item")));
            AssertEqual(expected, actual);
        }

        public void ItemActionAfterCloseOnceOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterCloseOnce = You close the item");
            var expected = new ItemAction(ItemActionType.AfterCloseOnce, new Section(new SectionText("You close the item")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionAfterOpenOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterOpen = You open the item");
            var expected = new ItemAction(ItemActionType.AfterOpen, new Section(new SectionText("You open the item")));
            AssertEqual(expected, actual);
        }

        public void ItemActionAfterOpenOnceOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterOpenOnce = You open the item");
            var expected = new ItemAction(ItemActionType.AfterOpenOnce, new Section(new SectionText("You open the item")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionAfterLockOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterLock = You lock the item");
            var expected = new ItemAction(ItemActionType.AfterLock, new Section(new SectionText("You lock the item")));
            AssertEqual(expected, actual);
        }

        public void ItemActionAfterLockOnceOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterLockOnce = You lock the item");
            var expected = new ItemAction(ItemActionType.AfterLockOnce, new Section(new SectionText("You lock the item")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionAfterUnlockOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterUnlock = You unlock the item");
            var expected = new ItemAction(ItemActionType.AfterUnlock, new Section(new SectionText("You unlock the item")));
            AssertEqual(expected, actual);
        }

        public void ItemActionAfterUnlockOnceOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterUnlockOnce = You unlock the item");
            var expected = new ItemAction(ItemActionType.AfterUnlockOnce, new Section(new SectionText("You unlock the item")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionAfterPickOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterPick = You pick the item");
            var expected = new ItemAction(ItemActionType.AfterPick, new Section(new SectionText("You pick the item")));
            AssertEqual(expected, actual);
        }

        public void ItemActionAfterPickOnceOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("afterPickOnce = You pick the item");
            var expected = new ItemAction(ItemActionType.AfterPickOnce, new Section(new SectionText("You pick the item")));
            AssertEqual(expected, actual);
        }
    }
}
