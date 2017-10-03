using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using Theseus.Parser;
using Theseus.Elements;

namespace TheseusTest
{
    [TestClass]
    public class FlagTests : TestBase
    {
        [TestMethod]
        public void FlagThatIsSetShouldBeParsedOK()
        {
            var actual = TheseusParser.FlagParser.Parse("flag foo is set");
            var expected = new Flag("foo", true);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void FlagThatIsNotSetShouldBeParsedOK()
        {
            var actual = TheseusParser.FlagParser.Parse("flag foo is not set");
            var expected = new Flag("foo", false);
            AssertEqual(expected, actual);
        }
    }
}
