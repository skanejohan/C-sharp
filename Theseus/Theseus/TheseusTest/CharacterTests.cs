using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprache;
using System.Collections.Generic;
using Theseus.Elements;
using Enum = Theseus.Elements.Enumerations;
using Theseus.Parser;

namespace TheseusTest
{
    [TestClass]
    public class CharacterTests : TestBase
    {
        [TestMethod]
        public void ConversationItemStatementDefinitionParsesOK()
        {
            var actual = TheseusParser.ConversationItemParser.Parse("statement 5 : Hello, there!");
            var expected = new ConversationItem(Enum.ConversationItemType.StatementDefinition, 5, 0, new Section(new SectionText("Hello, there!")), new List<int>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ConversationItemResponseDefinitionParsesOK()
        {
            var actual = TheseusParser.ConversationItemParser.Parse("response 5 : Hello, there!");
            var expected = new ConversationItem(Enum.ConversationItemType.ResponseDefinition, 5, 0, new Section(new SectionText("Hello, there!")), new List<int>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ConversationItemStatementWithOneResponseParsesOK()
        {
            var actual = TheseusParser.ConversationItemParser.Parse("statement 5 has response 3");
            var expected = new ConversationItem(Enum.ConversationItemType.StatementHasResponses, 5, 0, null, new List<int> { 3 });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ConversationItemStatementWithTwoResponsesParsesOK()
        {
            var actual = TheseusParser.ConversationItemParser.Parse("statement 5 has responses 3,4, 5");
            var expected = new ConversationItem(Enum.ConversationItemType.StatementHasResponses, 5, 0, null, new List<int> { 3, 4, 5 });
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ConversationItemResponseCausesParsesOK()
        {
            var actual = TheseusParser.ConversationItemParser.Parse("response 5 causes statement 3");
            var expected = new ConversationItem(Enum.ConversationItemType.ResponseCausesStatement, 5, 3, null, new List<int>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ConversationItemResponseEndsConversationParsesOK()
        {
            var actual = TheseusParser.ConversationItemParser.Parse("response 5 ends the conversation");
            var expected = new ConversationItem(Enum.ConversationItemType.ResponseEndsConversation, 5, 0, null, new List<int>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ConversationWithOneItemParsesOK()
        {
            var actual = TheseusParser.ConversationParser.Parse(
                @"conversation talk
                  -----------------
                  statement 1 : bla bla bla");
            var expected = new Conversation("talk",
                new List<ConversationItem>
                {
                    new ConversationItem(Enum.ConversationItemType.StatementDefinition, 1, 0, new Section(new SectionText("bla bla bla")), new List<int>())
                }); 

            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void ConversationWithManyItemsParsesOK()
        {
            var actual = TheseusParser.ConversationParser.Parse(
                @"conversation uncleTalk
                  ----------------------
                 statement 1: Hello Fiona, my dear!
                 response 1: Hello uncle Ailbert, how are you today?
                 statement 1 has response 1
                 response 1 causes statement 2
                 statement 2: Oh, the usual.
                 response 2: Aren't you seeing a doctor?
                 statement 2 has response 2
                 response 2 ends the conversation
                 ");
            var expected = new Conversation("uncleTalk",
                new List<ConversationItem>
                {
                    new ConversationItem(Enum.ConversationItemType.StatementDefinition, 1, 0, new Section(new SectionText("Hello Fiona, my dear!")), new List<int>()),
                    new ConversationItem(Enum.ConversationItemType.ResponseDefinition, 1, 0, new Section(new SectionText("Hello uncle Ailbert, how are you today?")), new List<int>()),
                    new ConversationItem(Enum.ConversationItemType.StatementHasResponses, 1, 0, null, new List<int> { 1 }),
                    new ConversationItem(Enum.ConversationItemType.ResponseCausesStatement, 1, 2, null, new List<int>()),
                    new ConversationItem(Enum.ConversationItemType.StatementDefinition, 2, 0, new Section(new SectionText("Oh, the usual.")), new List<int>()),
                    new ConversationItem(Enum.ConversationItemType.ResponseDefinition, 2, 0, new Section(new SectionText("Aren't you seeing a doctor?")), new List<int>()),
                    new ConversationItem(Enum.ConversationItemType.StatementHasResponses, 2, 0, null, new List<int> { 2 }),
                    new ConversationItem(Enum.ConversationItemType.ResponseEndsConversation, 2, 0, null, new List<int>()),
                });

            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterOptionStartsAtParsesOK()
        {
            var actual = TheseusParser.CharacterOptionParser.Parse("starts at trainStation");
            var expected = new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.StartsAt, "trainStation");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterOptionStartsInParsesOK()
        {
            var actual = TheseusParser.CharacterOptionParser.Parse("starts in kitchen");
            var expected = new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.StartsAt, "kitchen");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterOptionHiddenParsesOK()
        {
            var actual = TheseusParser.CharacterOptionParser.Parse("hidden");
            var expected = new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.Hidden);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterOptionFollowsPlayerStepParsesOK()
        {
            var actual = TheseusParser.CharacterOptionParser.Parse("follows player 1 step behind");
            var expected = new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.FollowsPlayerBehind, 1);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterOptionFollowsPlayerStepsParsesOK()
        {
            var actual = TheseusParser.CharacterOptionParser.Parse("follows player 15 steps behind");
            var expected = new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.FollowsPlayerBehind, 15);
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterOptionConversationIsParsesOK()
        {
            var actual = TheseusParser.CharacterOptionParser.Parse("conversation is uncleTalk");
            var expected = new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.ConversationIs, "uncleTalk");
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterParsesOK()
        {
            var actual = TheseusParser.CharacterParser.Parse(
                @"character uncleAilbert ""Uncle Ailbert""
                  ----------------------------------------
                  This is your uncle Ailbert.");
            var expected = new Character("uncleAilbert", "Uncle Ailbert",
                new List<CharacterOption>(),
                new Section(new SectionText("This is your uncle Ailbert.")),
                new List<Conversation>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterWithSpaceDelimitedOptionsParsesOK()
        {
            var actual = TheseusParser.CharacterParser.Parse(
                @"character uncleAilbert ""Uncle Ailbert"" starts at fictionSection hidden follows player 3 steps behind conversation is uncleTalk
                  ----------------------------------------
                  This is your uncle Ailbert.");
            var expected = new Character("uncleAilbert", "Uncle Ailbert",
                new List<CharacterOption>
                {
                    new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.StartsAt, "fictionSection"),
                    new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.Hidden),
                    new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.FollowsPlayerBehind, 3),
                    new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.ConversationIs, "uncleTalk")
                },
                new Section(new SectionText("This is your uncle Ailbert.")),
                new List<Conversation>());
            AssertEqual(expected, actual);
        }

        [TestMethod]
        public void CharacterWithCommaDelimitedOptionsParsesOK()
        {
            var actual = TheseusParser.CharacterParser.Parse(
                @"character uncleAilbert ""Uncle Ailbert"" starts at fictionSection hidden, follows player 3 steps behind, conversation is uncleTalk
                  ----------------------------------------
                  This is your uncle Ailbert.");
            var expected = new Character("uncleAilbert", "Uncle Ailbert",
                new List<CharacterOption>
                {
                    new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.StartsAt, "fictionSection"),
                    new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.Hidden),
                    new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.FollowsPlayerBehind, 3),
                    new CharacterOption(Theseus.Elements.Enumerations.CharacterOption.ConversationIs, "uncleTalk")
                },
                new Section(new SectionText("This is your uncle Ailbert.")),
                new List<Conversation>());
            AssertEqual(expected, actual);
        }
    }
}
