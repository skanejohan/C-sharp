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
        public void ItemActionParseOpenedOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("openedOk = You open the door");
            var expected = new ItemAction(ItemActionType.OpenedOk, new Section(new SectionText("You open the door")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionParseOpenFailed()
        {
            var actual = TheseusParser.ItemActionParser.Parse("openFailed = The door is locked");
            var expected = new ItemAction(ItemActionType.OpenFailed, new Section(new SectionText("The door is locked")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionParsePickedOK()
        {
            var actual = TheseusParser.ItemActionParser.Parse("pickedOk = You pick the door");
            var expected = new ItemAction(ItemActionType.PickedOk, new Section(new SectionText("You pick the door")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ItemActionParsePickFailed()
        {
            var actual = TheseusParser.ItemActionParser.Parse("pickFailed = You fail");
            var expected = new ItemAction(ItemActionType.PickFailed, new Section(new SectionText("You fail")));
            AssertEqual(expected, actual);
        }
    }
}
