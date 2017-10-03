using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System.Collections.Generic;
using Theseus.Parser;
using Theseus.Elements;
using Theseus.Interfaces;
using Theseus.Elements.Enumerations;

namespace TheseusTest
{
    [TestClass]
    public class SectionTests : TestBase
    {
        [TestMethod]
        public void TestTextBeforeIf()
        {
            var actual = TheseusParser.SectionParser.Parse("A<<if b is here>>B<<end>>");
            var expected = new Section(new List<IElement>
            {
                new SectionText("A"),
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsHere, "b", false)),
                    new Section(new SectionText("B"))))
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextAfterIf()
        {
            var actual = TheseusParser.SectionParser.Parse("<<if b is here>>B<<end>>C");
            var expected = new Section(new List<IElement>
            {
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsHere, "b", false)),
                    new Section(new SectionText("B")))),
                new SectionText("C")
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextBeforeAndAfterIf()
        {
            var actual = TheseusParser.SectionParser.Parse("A<<if b is here>>B<<end>>C");
            var expected = new Section(new List<IElement>
            {
                new SectionText("A"),
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsHere, "b", false)),
                    new Section(new SectionText("B")))),
                new SectionText("C"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestComplexIf()
        {
            var actual = TheseusParser.SectionParser.Parse("A<<if b is set>>B<<if c is not set>>!C<<end>>D<<end>>E");
            var expected = new Section(new List<IElement>
            {
                new SectionText("A"),
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsSet, "b", false)),
                    new Section(new List<IElement>
                    {
                        new SectionText("B"),
                        new IfStatement(new IfStatement.ConditionalSection(
                            new Expression(new SimpleCondition(ConditionMode.IsSet, "c", true)),
                            new Section(new SectionText("!C"))
                        )),
                        new SectionText("D")
                    }))),
                new SectionText("E"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextBeforeIfElse()
        {
            var actual = TheseusParser.SectionParser.Parse("before<<if a is set>>A<<else>>B<<end>>");
            var expected = new Section(new List<IElement>
            {
                new SectionText("before"),
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                    new Section(new SectionText("A"))),
                    new Section(new SectionText("B")))
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextAfterIfElse()
        {
            var actual = TheseusParser.SectionParser.Parse("<<if a is set>>A<<else>>B<<end>>after");
            var expected = new Section(new List<IElement>
            {
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                    new Section(new SectionText("A"))),
                    new Section(new SectionText("B"))),
                new SectionText("after")
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextBeforeAndAfterIfElse()
        {
            var actual = TheseusParser.SectionParser.Parse("before<<if a is set>>A<<else>>B<<end>>after");
            var expected = new Section(new List<IElement>
            {
                new SectionText("before"),
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                    new Section(new SectionText("A"))),
                    new Section(new SectionText("B"))),
                new SectionText("after")
            });
            AssertEqual(expected, actual);
        }


        [TestMethod]
        public void TestComplexIfElse()
        {
            var actual = TheseusParser.SectionParser.Parse("before<<if a is set>>A<<else>><<if upper is set>>B<<else>>b<<end>><<end>>after");
            var expected = new Section(new List<IElement>
            {
                new SectionText("before"),
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsSet, "a", false)),
                    new Section(new SectionText("A"))),
                    new Section(new List<IElement>
                    {
                        new IfStatement(new IfStatement.ConditionalSection(
                            new Expression(new SimpleCondition(ConditionMode.IsSet, "upper", false)),
                            new Section(new SectionText("B"))),
                            new Section(new SectionText("b")))
                    })),
                    new SectionText("after")
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextBeforeAction()
        {
            var actual = TheseusParser.SectionParser.Parse("A[[set a]]");
            var expected = new Section(new List<IElement>
            {
                new SectionText("A"),
                new Action(new Effect(EffectType.Set, "a", ""))
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextAfterAction()
        {
            var actual = TheseusParser.SectionParser.Parse("[[set a]]B");
            var expected = new Section(new List<IElement>
            {
                new Action(new Effect(EffectType.Set, "a", "")),
                new SectionText("B")
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextBeforeAndAfterAction()
        {
            var actual = TheseusParser.SectionParser.Parse("A[[set a]]B");
            var expected = new Section(new List<IElement>
            {
                new SectionText("A"),
                new Action(new Effect(EffectType.Set, "a", "")),
                new SectionText("B"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextWithSpaces()
        {
            var actual = TheseusParser.SectionParser.Parse("a b c d e");
            var expected = new Section(new List<IElement>
            {
                new SectionText("a b c d e"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextWithSpacesAndLineBreak()
        {
            var actual = TheseusParser.SectionParser.Parse("a b c d e\nf g h i j");
            var expected = new Section(new List<IElement>
            {
                new SectionText("a b c d e"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextWithStartingHtmlBold()
        {
            var actual = TheseusParser.SectionParser.Parse("a<b>bc");
            var expected = new Section(new List<IElement>
            {
                new SectionText("a<b>bc"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextWithEndingHtmlBold()
        {
            var actual = TheseusParser.SectionParser.Parse("a</b>bc");
            var expected = new Section(new List<IElement>
            {
                new SectionText("a</b>bc"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextWithStartingHtmlItalic()
        {
            var actual = TheseusParser.SectionParser.Parse("a<i>bc");
            var expected = new Section(new List<IElement>
            {
                new SectionText("a<i>bc"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextWithEndingHtmlItalic()
        {
            var actual = TheseusParser.SectionParser.Parse("a</i>bc");
            var expected = new Section(new List<IElement>
            {
                new SectionText("a</i>bc"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextWithHtmlLineBreak()
        {
            var actual = TheseusParser.SectionParser.Parse("a<br>b<br>c");
            var expected = new Section(new List<IElement>
            {
                new SectionText("a<br>b<br>c"),
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestTextWithHtmlLineBreakAfterIf()
        {
            var actual = TheseusParser.SectionParser.Parse("<<if b is here>>B<<end>><br>C");
            var expected = new Section(new List<IElement>
            {
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsHere, "b", false)),
                    new Section(new SectionText("B")))),
                new SectionText("<br>C")
            });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void TestSpacesAndIf()
        {
            var actual = TheseusParser.SectionParser.Parse("<<if b is here>> B <<end>> C ");
            var expected = new Section(new List<IElement>
            {
                new IfStatement(new IfStatement.ConditionalSection(
                    new Expression(new SimpleCondition(ConditionMode.IsHere, "b", false)),
                    new Section(new SectionText(" B ")))),
                new SectionText(" C ")
            });
            AssertEqual(expected, actual);
        }
    }
}
