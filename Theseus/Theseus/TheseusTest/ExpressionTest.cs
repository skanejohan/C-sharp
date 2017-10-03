using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using Theseus.Elements;
using Theseus.Elements.Enumerations;
using Theseus.Parser;

namespace TheseusTest
{
    [TestClass]
    public class ExpressionTest : TestBase
    {

        [TestMethod]
        public void ExpressionParsesOk()
        {
            var actual = TheseusParser.ComplexConditionParser.Parse("a is open");
            var expected = new Expression(new SimpleCondition(ConditionMode.IsState, "a", ItemState.Open, false));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void AndParsesOk()
        {
            var actual = TheseusParser.ComplexConditionParser.Parse("a is open and b is closed");
            var expected = new And(
                new SimpleCondition(ConditionMode.IsState, "a", ItemState.Open, false),
                new SimpleCondition(ConditionMode.IsState, "b", ItemState.Closed, false));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void OrParsesOk()
        {
            var actual = TheseusParser.ComplexConditionParser.Parse("a is open or b is closed");
            var expected = new Or(
                new SimpleCondition(ConditionMode.IsState, "a", ItemState.Open, false),
                new SimpleCondition(ConditionMode.IsState, "b", ItemState.Closed, false));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void NotParsesOk()
        {
            var actual = TheseusParser.ComplexConditionParser.Parse("not a is open");
            var expected = new Not(new SimpleCondition(ConditionMode.IsState, "a", ItemState.Open, false));
            AssertEqual(expected, actual);
        }
    }
}
