using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theseus.Parser;
using Sprache;
using Theseus.Elements;
using Theseus.Elements.Enumerations;

namespace TheseusTest
{
    [TestClass]
    public class IfTest : TestBase
    {
        [TestMethod]
        public void IfIsStateParsesOK()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is open>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsState, "a", ItemState.Open, false)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIsNotState()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is not open>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsState, "a", ItemState.Open, true)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIsSet()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is set>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIsNotSet()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is not set>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsSet, "a", true)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfHasBeenState()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a has been open>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.HasBeenState, "a", ItemState.Open, false)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfHasNotBeenState()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a has not been open>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.HasBeenState, "a", ItemState.Open, true)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfHasBeenSet()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a has been set>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.HasBeenSet, "a", false)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfHasNotBeenSet()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a has not been set>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.HasBeenSet, "a", true)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIsIn()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is in b>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsIn, "a", "b", false)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIsNotIn()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is not in b>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsIn, "a", "b", true)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIsHere()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is here>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsHere, "a", false)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIsNotHere()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is not here>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsHere, "a", true)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfICarry()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if I carry a>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsCarried, "a", false)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIDontCarry()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if I do not carry a>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsCarried, "a", true)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIHaveCarried()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if I have carried a>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.HasBeenCarried, "a", false)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestIfIHaventCarried()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if I have not carried a>>A<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.HasBeenCarried, "a", true)),
                new Section(new SectionText("A"))));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void IfElseParsesOK()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is set>>A<<else>>B<<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                new Section(new SectionText("A"))),
                new Section(new SectionText("B")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void IfElseWithSpacesParsesOK()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is set>> A <<else>> B <<end>>");
            var expected = new IfStatement(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                new Section(new SectionText(" A "))),
                new Section(new SectionText(" B ")));
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void IfElseIfParsesOK()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is set>>A<<else if b is set>>B<<end>>");
            var expected = new IfStatement();
            expected.AddExpression(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                new Section(new SectionText("A")))
            );
            expected.AddExpression(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsSet, "b", false)),
                new Section(new SectionText("B")))
            );
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void IfElseIfElseParsesOK()
        {
            var actual = TheseusParser.IfStatementParser.Parse("<<if a is set>>A<<else if b is set>>B<<else>>C<<end>>");
            var expected = new IfStatement();
            expected.AddExpression(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                new Section(new SectionText("A")))
            );
            expected.AddExpression(new IfStatement.ConditionalSection(
                new Expression(new SimpleCondition(ConditionMode.IsSet, "b", false)),
                new Section(new SectionText("B")))
            );
            expected.DefaultSection = new Section(new SectionText("C"));
            AssertEqual(expected, actual);
        }
    }
}
