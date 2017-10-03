using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Theseus.Parser;
using Theseus.Elements;
using Theseus.Interfaces;

namespace TheseusTest
{
    public class TestBase
    {
        protected void AssertEqual(IEnumerable<IElement> expected, IEnumerable<IElement> actual)
        {
            var a = actual.ToList();
            var e = expected.ToList();

            Assert.AreEqual(e.Count(), a.Count());

            for (var i = 0; i < e.Count(); i++)
            {
                AssertEqual(e[i], a[i]);
            }
        }

        protected void AssertEqualTupleList(IEnumerable<System.Tuple<IElement,IElement>> expected, IEnumerable<System.Tuple<IElement, IElement>> actual)
        {
            var a = actual.ToList();
            var e = expected.ToList();

            Assert.AreEqual(e.Count(), a.Count());

            for (var i = 0; i < e.Count(); i++)
            {
                AssertEqual(e[i].Item1, a[i].Item1);
                AssertEqual(e[i].Item2, a[i].Item2);
            }
        }

        private void AssertEqual(SimpleCondition expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(SimpleCondition));
            var _actual = actual as SimpleCondition;

            Assert.AreEqual(expected.Host, _actual.Host);
            Assert.AreEqual(expected.Invert, _actual.Invert);
            Assert.AreEqual(expected.Mode, _actual.Mode);
            Assert.AreEqual(expected.Object, _actual.Object);
            Assert.AreEqual(expected.State, _actual.State);
        }

        private void AssertEqual(Expression expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Expression));
            var _actual = actual as Expression;
            AssertEqual(expected.Condition, _actual.Condition);
        }

        private void AssertEqual(Or expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Or));
            var _actual = actual as Or;
            AssertEqual(expected.Condition, _actual.Condition);
            AssertEqual(expected.Condition2, _actual.Condition2);
        }

        private void AssertEqual(And expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(And));
            var _actual = actual as And;
            AssertEqual(expected.Condition, _actual.Condition);
            AssertEqual(expected.Condition2, _actual.Condition2);
        }

        private void AssertEqual(Not expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Not));
            var _actual = actual as Not;
            AssertEqual(expected.Condition, _actual.Condition);
        }

        private void AssertEqual(IfStatement expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(IfStatement));
            var _actual = actual as IfStatement;

            var a = _actual.Expressions.ToList();
            var e = expected.Expressions.ToList();

            Assert.AreEqual(e.Count(), a.Count());

            for (var i = 0; i < e.Count(); i++)
            {
                AssertEqual(e[i].Expression, a[i].Expression);
                AssertEqual(e[i].Section, a[i].Section);
            }
            AssertEqual(expected.DefaultSection, _actual.DefaultSection);
        }

        private void AssertEqual(SectionText expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(SectionText));
            var _actual = actual as SectionText;

            Assert.AreEqual(expected.Txt, _actual.Txt);
        }

        private void AssertEqual(Section expected, IElement actual)
        {
            if (actual == null && expected == null)
            {
                return;
            }

            Assert.IsInstanceOfType(actual, typeof(Section));
            var _actual = actual as Section;

            var actualElements = _actual.Elements.ToList();
            var expectedElements = expected.Elements.ToList();
            Assert.AreEqual(expectedElements.Count(), actualElements.Count());

            for (var i = 0; i < expected.Elements.Count(); i++)
            {
                AssertEqual(expectedElements[i], actualElements[i]);
            }
        }

        private void AssertEqual(Effect expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Effect));
            var _actual = actual as Effect;

            Assert.AreEqual(expected.Type, _actual.Type);
            Assert.AreEqual(expected.Object, _actual.Object);
            Assert.AreEqual(expected.Host, _actual.Host);
        }

        private void AssertEqual(Action expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Action));
            var _actual = actual as Action;

            var actualEffects = _actual.Effects.ToList();
            var expectedEffects = expected.Effects.ToList();
            Assert.AreEqual(expectedEffects.Count(), actualEffects.Count());

            for (var i = 0; i < expected.Effects.Count(); i++)
            {
                AssertEqual(expectedEffects[i], actualEffects[i]);
            }
        }

        private void AssertEqual(ItemOption expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(ItemOption));
            var _actual = actual as ItemOption;
            Assert.AreEqual(expected.Type, _actual.Type);
            Assert.AreEqual(expected.Data, _actual.Data);
        }

        private void AssertEqual(ItemAction expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(ItemAction));
            var _actual = actual as ItemAction;
            Assert.AreEqual(expected.Type, _actual.Type);
            AssertEqual(expected.Section, _actual.Section);
        }

        private void AssertEqual(Item expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Item));
            var _actual = actual as Item;
            Assert.AreEqual(expected.Level, _actual.Level);
            Assert.AreEqual(expected.Name, _actual.Name);
            Assert.AreEqual(expected.Label, _actual.Label);
            AssertEqual(expected.Actions, _actual.Actions);
            AssertEqual(expected.Functions, _actual.Functions);
            AssertEqual(expected.Options, _actual.Options);
            AssertEqual(expected.Section, _actual.Section);
        }

        private void AssertEqual(Function expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Function));
            var _actual = actual as Function;
            Assert.AreEqual(expected.Name, _actual.Name);
            Assert.AreEqual(expected.Label, _actual.Label);
            Assert.AreEqual(expected.Hidden, _actual.Hidden);
            AssertEqual(expected.Section, _actual.Section);
        }

        private void AssertEqual(Door expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Door));
            var _actual = actual as Door;
            Assert.AreEqual(expected.Name, _actual.Name);
            Assert.AreEqual(expected.Label, _actual.Label);
            AssertEqual(expected.Options, _actual.Options);
        }

        private void AssertEqual(Exit expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Exit));
            var _actual = actual as Exit;
            Assert.AreEqual(expected.Direction, _actual.Direction);
            Assert.AreEqual(expected.Door, _actual.Door);
            Assert.AreEqual(expected.Target, _actual.Target);
        }

        private void AssertEqual(Flag expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Flag));
            var _actual = actual as Flag;
            Assert.AreEqual(expected.Name, _actual.Name);
            Assert.AreEqual(expected.Set, _actual.Set);
        }

        private void AssertEqual(ConversationItem expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(ConversationItem));
            var _actual = actual as ConversationItem;
            Assert.AreEqual(expected.Type, _actual.Type);
            Assert.AreEqual(expected.Number, _actual.Number);
            Assert.AreEqual(expected.CausesNumber, _actual.CausesNumber);
            AssertEqual(expected.Section, _actual.Section);

            var e = expected.Responses.ToList();
            var a = _actual.Responses.ToList();
            Assert.AreEqual(e.Count(), a.Count());

            for (var i = 0; i < e.Count(); i++)
            {
                Assert.AreEqual(e[i], a[i]);
            }
        }

        private void AssertEqual(Conversation expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Conversation));
            var _actual = actual as Conversation;
            Assert.AreEqual(expected.Name, _actual.Name);
            AssertEqual(expected.ConversationItems, _actual.ConversationItems);
        }

        private void AssertEqual(CharacterOption expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(CharacterOption));
            var _actual = actual as CharacterOption;
            Assert.AreEqual(expected.Ident, _actual.Ident);
            Assert.AreEqual(expected.Option, _actual.Option);
            Assert.AreEqual(expected.StepsBehind, _actual.StepsBehind);
        }

        private void AssertEqual(Character expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Character));
            var _actual = actual as Character;
            AssertEqual(expected.CharacterOptions, _actual.CharacterOptions);
            AssertEqual(expected.Conversations, _actual.Conversations);
            Assert.AreEqual(expected.Label, _actual.Label);
            Assert.AreEqual(expected.Name, _actual.Name);
            AssertEqual(expected.Section, _actual.Section);
        }

        private void AssertEqual(Location expected, IElement actual)
        {
            Assert.IsInstanceOfType(actual, typeof(Location));
            var _actual = actual as Location;
            AssertEqual(expected.Exits, _actual.Exits);
            AssertEqual(expected.Flags, _actual.Flags);
            Assert.AreEqual(expected.Label, _actual.Label);
            Assert.AreEqual(expected.Name, _actual.Name);
            AssertEqual(expected.Section, _actual.Section);
            AssertEqual(expected.Items, _actual.Items);
            AssertEqual(expected.Doors, _actual.Doors);
        }

        protected void AssertEqual(IElement expected, IElement actual)
        {
            if (expected is SimpleCondition)
            {
                AssertEqual((SimpleCondition)expected, actual);
            }
            else if (expected is IfStatement)
            {
                AssertEqual((IfStatement)expected, actual);
            }
            else if (expected is SectionText)
            {
                AssertEqual((SectionText)expected, actual);
            }
            else if (expected is Section)
            {
                AssertEqual((Section)expected, actual);
            }
            else if (expected is Effect)
            {
                AssertEqual((Effect)expected, actual);
            }
            else if (expected is Action)
            {
                AssertEqual((Action)expected, actual);
            }
            else if (expected is ItemOption)
            {
                AssertEqual((ItemOption)expected, actual);
            }
            else if (expected is ItemAction)
            {
                AssertEqual((ItemAction)expected, actual);
            }
            else if (expected is Item)
            {
                AssertEqual((Item)expected, actual);
            }
            else if (expected is Function)
            {
                AssertEqual((Function)expected, actual);
            }
            else if (expected is Door)
            {
                AssertEqual((Door)expected, actual);
            }
            else if (expected is Flag)
            {
                AssertEqual((Flag)expected, actual);
            }
            else if (expected is Exit)
            {
                AssertEqual((Exit)expected, actual);
            }
            else if (expected is ConversationItem)
            {
                AssertEqual((ConversationItem)expected, actual);
            }
            else if (expected is Conversation)
            {
                AssertEqual((Conversation)expected, actual);
            }
            else if (expected is CharacterOption)
            {
                AssertEqual((CharacterOption)expected, actual);
            }
            else if (expected is Character)
            {
                AssertEqual((Character)expected, actual);
            }
            else if (expected is Location)
            {
                AssertEqual((Location)expected, actual);
            }
            else if (expected is And)
            {
                AssertEqual((And)expected, actual);
            }
            else if (expected is Or)
            {
                AssertEqual((Or)expected, actual);
            }
            else if (expected is Not)
            {
                AssertEqual((Not)expected, actual);
            }
            else if (expected is Expression)
            {
                AssertEqual((Expression)expected, actual);
            }
            else
            {
                Assert.Fail("Test does not support element type " + expected.GetType());
            }
        }
    }
}
