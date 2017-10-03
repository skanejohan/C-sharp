using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using Theseus.Elements;
using Theseus.Parser;

namespace TheseusTest
{
    [TestClass]
    public class FunctionTests : TestBase
    {
        [TestMethod]
        public void TestFunction()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open\" As you move toward...");
            var expected = new Function("open", "Open", false, new Section(new SectionText("As you move toward...")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestFunctionWithSentenceAsLabel()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open this one\" As you move toward...");
            var expected = new Function("open", "Open this one", false, new Section(new SectionText("As you move toward...")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestFunctionWithDashes()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open\" -------- As you move toward...");
            var expected = new Function("open", "Open", false, new Section(new SectionText("As you move toward...")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestFunctionWithLineBreaks()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open\"\nAs you move\ntoward...");
            var expected = new Function("open", "Open", false, new Section(new SectionText("As you move")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestFunctionWithDashesAndLineBreaks()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open\"\n----------\nAs you move\ntoward...");
            var expected = new Function("open", "Open", false, new Section(new SectionText("As you move")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestHiddenFunction()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open\" hidden As you move toward...");
            var expected = new Function("open", "Open", true, new Section(new SectionText("As you move toward...")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestHiddenFunctionWithSentenceAsLabel()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open this one\" hidden As you move toward...");
            var expected = new Function("open", "Open this one", true, new Section(new SectionText("As you move toward...")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestHiddenFunctionWithDashes()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open\" hidden -------- As you move toward...");
            var expected = new Function("open", "Open", true, new Section(new SectionText("As you move toward...")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestHiddenFunctionWithLineBreaks()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open\" hidden \nAs you move\ntoward...");
            var expected = new Function("open", "Open", true, new Section(new SectionText("As you move")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestHiddenFunctionWithDashesAndLineBreaks()
        {
            var actual = TheseusParser.FunctionParser.Parse("function open \"Open\" hidden\n ----------\nAs you move\ntoward...");
            var expected = new Function("open", "Open", true, new Section(new SectionText("As you move")));
            AssertEqual(expected, actual);
        }
    }
}
