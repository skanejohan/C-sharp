using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theseus.Parser;
using Sprache;
using Theseus.Elements.Enumerations;
using Theseus.Elements;

namespace TheseusTest
{
    [TestClass]
    public class ExitTests : TestBase
    {
        [TestMethod]
        public void ExitShouldParseNorthOK()
        {
            var actual = TheseusParser.ExitParser.Parse("exit north to kitchen");
            var expected = new Exit(Direction.N, "kitchen", "");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ExitShouldParseEastOK()
        {
            var actual = TheseusParser.ExitParser.Parse("exit east to kitchen");
            var expected = new Exit(Direction.E, "kitchen", "");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ExitShouldParseSouthOK()
        {
            var actual = TheseusParser.ExitParser.Parse("exit south to kitchen");
            var expected = new Exit(Direction.S, "kitchen", "");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ExitShouldParseWestOK()
        {
            var actual = TheseusParser.ExitParser.Parse("exit west to kitchen");
            var expected = new Exit(Direction.W, "kitchen", "");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ExitShouldParseViaOK()
        {
            var actual = TheseusParser.ExitParser.Parse("exit north to kitchen via hall");
            var expected = new Exit(Direction.N, "kitchen", "hall");
            AssertEqual(expected, actual);
        }
    }
}
