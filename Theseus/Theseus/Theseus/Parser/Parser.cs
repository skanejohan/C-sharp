using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Theseus.Elements;
using Enum = Theseus.Elements.Enumerations;
using Theseus.Interfaces;

[assembly: InternalsVisibleTo("TheseusTest")]

namespace Theseus.Parser
{
    public static partial class TheseusParser
    {
        public static IElement ParseDocument(string s)
        {
            return DocumentParser.Parse(s);
        }

        internal static readonly Parser<char> CommaParser = Parse.Chars(",");

        internal static readonly Parser<IEnumerable<int>> NumbersParser =
            from nums in Parse.Number.Token().DelimitedBy(CommaParser)
            select nums.Select(s => int.Parse(s));

        internal static readonly Parser<IEnumerable<char>> DashesParser = Parse.Char('-').Many().Token();

        internal static readonly Parser<string> QuotedStringParser =
            from _1 in Parse.String("\"")
            from s in Parse.AnyChar.Except(Parse.Char('"')).Many().Text()
            from _2 in Parse.String("\"")
            select s;

        internal static readonly Parser<char> LineBreakParser = Parse.Chars(Environment.NewLine);

        internal static readonly Parser<char> SeparatorCharParser = Parse.Chars("()<>@,;:\\\"/[]?={} \t");

        internal static readonly Parser<char> ControlCharParser = Parse.Char(char.IsControl, "Control character");

        internal static readonly Parser<char> IdentCharParser = Parse.AnyChar.Except(SeparatorCharParser).Except(ControlCharParser);

        internal static readonly Parser<string> IdentParser = IdentCharParser.AtLeastOnce().Text().Token();

        internal static readonly Parser<Enum.ItemState> StateParser =
            (Parse.String("open").Return(Enum.ItemState.Open))
            .Or(Parse.String("closed").Return(Enum.ItemState.Closed))
            .Or(Parse.String("locked").Return(Enum.ItemState.Locked))
            .Or(Parse.String("unlocked").Return(Enum.ItemState.Unlocked))
            .Or(Parse.String("picked").Return(Enum.ItemState.Picked))
            .Or(Parse.String("empty").Return(Enum.ItemState.Empty))
            .Or(Parse.String("hidden").Return(Enum.ItemState.Hidden));

        internal static readonly Parser<SimpleCondition> SimpleConditionParser =
            (from ident in IdentParser
             from _1 in Parse.String("is").Token()
             from _2 in Parse.String("set").Token()
             select new SimpleCondition(Enum.ConditionMode.IsSet, ident, false))
            .Or(from ident in IdentParser
                from _1 in Parse.String("is").Token()
                from _2 in Parse.String("not").Token()
                from _3 in Parse.String("set").Token()
                select new SimpleCondition(Enum.ConditionMode.IsSet, ident, true))
            .Or(from ident in IdentParser
                from _1 in Parse.String("is").Token()
                from state in StateParser
                select new SimpleCondition(Enum.ConditionMode.IsState, ident, state, false))
            .Or(from ident in IdentParser
                from _1 in Parse.String("is").Token()
                from _2 in Parse.String("not").Token()
                from state in StateParser
                select new SimpleCondition(Enum.ConditionMode.IsState, ident, state, true))
            .Or(from ident in IdentParser
                from _1 in Parse.String("has").Token()
                from _2 in Parse.String("been").Token()
                from _3 in Parse.String("set").Token()
                select new SimpleCondition(Enum.ConditionMode.HasBeenSet, ident, false))
            .Or(from ident in IdentParser
                from _1 in Parse.String("has").Token()
                from _2 in Parse.String("not").Token()
                from _3 in Parse.String("been").Token()
                from _4 in Parse.String("set").Token()
                select new SimpleCondition(Enum.ConditionMode.HasBeenSet, ident, true))
            .Or(from ident in IdentParser
                from _1 in Parse.String("has").Token()
                from _2 in Parse.String("been").Token()
                from state in StateParser
                select new SimpleCondition(Enum.ConditionMode.HasBeenState, ident, state, false))
            .Or(from ident in IdentParser
                from _1 in Parse.String("has").Token()
                from _2 in Parse.String("not").Token()
                from _3 in Parse.String("been").Token()
                from state in StateParser
                select new SimpleCondition(Enum.ConditionMode.HasBeenState, ident, state, true))
            .Or(from obj in IdentParser
                from _1 in Parse.String("is").Token()
                from _2 in Parse.String("in").Token()
                from host in IdentParser
                select new SimpleCondition(Enum.ConditionMode.IsIn, obj, host, false))
            .Or(from obj in IdentParser
                from _1 in Parse.String("is").Token()
                from _2 in Parse.String("not").Token()
                from _3 in Parse.String("in").Token()
                from host in IdentParser
                select new SimpleCondition(Enum.ConditionMode.IsIn, obj, host, true))
            .Or(from obj in IdentParser
                from _1 in Parse.String("is").Token()
                from _2 in Parse.String("here").Token()
                select new SimpleCondition(Enum.ConditionMode.IsHere, obj, false))
            .Or(from obj in IdentParser
                from _1 in Parse.String("is").Token()
                from _2 in Parse.String("not").Token()
                from _3 in Parse.String("here").Token()
                select new SimpleCondition(Enum.ConditionMode.IsHere, obj, true))
            .Or(from _1 in Parse.String("I").Token()
                from _2 in Parse.String("carry").Token()
                from obj in IdentParser
                select new SimpleCondition(Enum.ConditionMode.IsCarried, obj, false))
            .Or(from _1 in Parse.String("I").Token()
                from _2 in Parse.String("do").Token()
                from _3 in Parse.String("not").Token()
                from _4 in Parse.String("carry").Token()
                from obj in IdentParser
                select new SimpleCondition(Enum.ConditionMode.IsCarried, obj, true))
            .Or(from _1 in Parse.String("I").Token()
                from _2 in Parse.String("have").Token()
                from _3 in Parse.String("carried").Token()
                from obj in IdentParser
                select new SimpleCondition(Enum.ConditionMode.HasBeenCarried, obj, false))
            .Or(from _1 in Parse.String("I").Token()
                from _2 in Parse.String("have").Token()
                from _3 in Parse.String("not").Token()
                from _4 in Parse.String("carried").Token()
                from obj in IdentParser
                select new SimpleCondition(Enum.ConditionMode.HasBeenCarried, obj, true));

        internal static readonly Parser<Expression> ConditionFactorParser =
            (from _1 in Parse.String("(").Token()
             from e in ComplexConditionParser
             from _2 in Parse.String(")").Token()
             select e).Or(
                from _1 in Parse.String("not").Token()
                from e in ComplexConditionParser
                select new Not(e.Condition)).Or(
                        from e in SimpleConditionParser
                        select new Expression(e));

        internal static readonly Parser<Expression> ConditionTermParser =
            (from e1 in ConditionFactorParser
             from _1 in Parse.String("and").Token()
             from e2 in ConditionTermParser
             select new And(e1.Condition, e2.Condition)).Or(
                from e in ConditionFactorParser
                select e);

        internal static readonly Parser<Expression> ComplexConditionParser =
            (from e1 in ConditionTermParser
             from _1 in Parse.String("or").Token()
             from e2 in ComplexConditionParser
             select new Or(e1.Condition, e2.Condition)).Or(
                from e in ConditionTermParser
                select e);

        internal static readonly Parser<IfStatement.ConditionalSection> IfParser =
            from _1 in Parse.String("<<").Token()
            from _2 in Parse.String("if").Token()
            from expression in ComplexConditionParser
            from _3 in Parse.String(">>")
            from section in SectionParser
            select new IfStatement.ConditionalSection(expression, section);

        internal static readonly Parser<IfStatement.ConditionalSection> ElseIfParser =
            from _1 in Parse.String("<<").Token()
            from _2 in Parse.String("else").Token()
            from _3 in Parse.String("if").Token()
            from expression in ComplexConditionParser
            from _4 in Parse.String(">>").Token()
            from section in SectionParser
            select new IfStatement.ConditionalSection(expression, section);

        internal static readonly Parser<Section> ElseParser =
            from o in Parse.String("<<").Token()
            from e in Parse.String("else").Token()
            from c in Parse.String(">>")
            from section in SectionParser
            select section;

        internal static readonly Parser<int> EndParser =
            from o in Parse.String("<<").Token()
            from e in Parse.String("end").Token()
            from c in Parse.String(">>")
            select 0;

        internal static readonly Parser<IfStatement> IfStatementParser =
            (from _if in IfParser
             from elseifs in ElseIfParser.Many()
             from _1 in EndParser
             select new IfStatement(_if, elseifs))
            .Or(
                from _if in IfParser
                from elseifs in ElseIfParser.Many()
                from _else in ElseParser
                from _1 in EndParser
                select new IfStatement(_if, elseifs, _else));

        internal static readonly Parser<Effect> EffectParser =
            (from _1 in Parse.String("add").Token()
             from obj in IdentParser
             from _2 in Parse.String("to").Token()
             from host in IdentParser
             select new Effect(Enum.EffectType.Add, obj, host))
            .Or(from _1 in Parse.String("set").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.Set, obj))
            .Or(from _1 in Parse.String("clear").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.Clear, obj))
            .Or(from _1 in Parse.String("show").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.Show, obj))
            .Or(from _1 in Parse.String("hide").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.Hide, obj))
            .Or(from _1 in Parse.String("move").Token()
                from _2 in Parse.String("to").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.MoveTo, obj))
            .Or(from _1 in Parse.String("lock").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.Lock, obj))
            .Or(from _1 in Parse.String("unlock").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.Unlock, obj))
            .Or(from _1 in Parse.String("close").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.Close, obj))
            .Or(from _1 in Parse.String("open").Token()
                from obj in IdentParser
                select new Effect(Enum.EffectType.Open, obj));

        internal static readonly Parser<Theseus.Elements.Action> ActionParser =
            from _1 in Parse.String("[[").Token()
            from effects in EffectParser.DelimitedBy(CommaParser)
            from _2 in Parse.String("]]")
            select new Theseus.Elements.Action(effects);

        internal static readonly Parser<string> SectionTextStringParser =
            (from t in (Parse.AnyChar.Except(LineBreakParser).Except(Parse.Char('<')).Except(Parse.Char('[')))
             select t.ToString())
            .Or(Parse.String("<br>").Text())
            .Or(Parse.String("<b>").Text())
            .Or(Parse.String("</b>").Text())
            .Or(Parse.String("<i>").Text())
            .Or(Parse.String("</i>").Text());

        internal static readonly Parser<IElement> SectionTextParser =
            from t in SectionTextStringParser.AtLeastOnce()
            select new SectionText(string.Join("", t));

        internal static readonly Parser<IElement> SectionItemParser =
            IfStatementParser.Or(SectionTextParser).Or(ActionParser);

        internal static readonly Parser<Section> SectionParser =
            from sections in SectionItemParser.AtLeastOnce()
            select new Section(sections);

        internal static readonly Parser<Function> FunctionParser =
            from _1 in Parse.String("function").Token()
            from name in IdentParser.Token()
            from label in QuotedStringParser.Token()
            from hidden in Parse.Optional(Parse.String("hidden").Token())
            from _3 in DashesParser
            from section in SectionParser
            select new Function(name, label, hidden.IsDefined, section);

        internal static readonly Parser<Enum.ItemActionType> ItemActionTypeParser =
            Parse.String("openedOk").Return(Enum.ItemActionType.OpenedOk)
            .Or(Parse.String("openFailed").Return(Enum.ItemActionType.OpenFailed))
            .Or(Parse.String("pickedOk").Return(Enum.ItemActionType.PickedOk))
            .Or(Parse.String("pickFailed").Return(Enum.ItemActionType.PickFailed))
            .Or(Parse.String("afterTake").Return(Enum.ItemActionType.AfterTake));

        internal static readonly Parser<ItemAction> ItemActionParser =
            from at in ItemActionTypeParser.Token()
            from _1 in Parse.String("=").Token()
            from section in SectionParser
            select new ItemAction(at, section);

        internal static readonly Parser<ItemOption> RequiresCombinationParser =
            from _1 in Parse.String("requires").Token()
            from _2 in Parse.String("combination").Token()
            from num in Parse.Number
            select new ItemOption(Enum.ItemOptionType.RequiresCombination, num);

        internal static readonly Parser<ItemOption> RequiresKeyParser =
            from _1 in Parse.String("requires").Token()
            from _2 in Parse.String("key").Token()
            from key in IdentParser
            select new ItemOption(Enum.ItemOptionType.RequiresKey, key);

        internal static readonly Parser<ItemOption> DoorOptionParser =
            Parse.String("openable").Return(new ItemOption(Enum.ItemOptionType.Openable))
            .Or(Parse.String("opensWhenPicked").Return(new ItemOption(Enum.ItemOptionType.OpensWhenPicked)))
            .Or(Parse.String("open").Return(new ItemOption(Enum.ItemOptionType.Open)))
            .Or(Parse.String("closed").Return(new ItemOption(Enum.ItemOptionType.Closed)))
            .Or(Parse.String("lockable").Return(new ItemOption(Enum.ItemOptionType.Lockable)))
            .Or(Parse.String("locked").Return(new ItemOption(Enum.ItemOptionType.Locked)))
            .Or(Parse.String("unlocked").Return(new ItemOption(Enum.ItemOptionType.Unlocked)))
            .Or(Parse.String("pickable").Return(new ItemOption(Enum.ItemOptionType.Pickable)))
            .Or(RequiresCombinationParser)
            .Or(RequiresKeyParser);

        internal static readonly Parser<ItemOption> ItemOptionParser =
            DoorOptionParser.Or(Parse.String("container").Return(new ItemOption(Enum.ItemOptionType.Container)));

        internal static readonly Parser<int> PlusParser =
            from s in Parse.Char('+').Many().Text().Token()
            select s.Length;

        internal static readonly Parser<Item> ItemParser =
            from level in PlusParser
            from _1 in Parse.String("item")
            from name in IdentParser
            from text in QuotedStringParser.Token()
            from hidden in Parse.Optional(Parse.String("hidden"))
            from options in ItemOptionParser.Token().Many().Token()
            from _2 in DashesParser
            from section in SectionParser
            from functions in FunctionParser.Token().Many().Token()
            from actions in ItemActionParser.Token().Many().Token()
            select new Item(level, name, text, hidden.IsDefined, options, section, functions, actions);

        internal static readonly Parser<Door> DoorParser =
            from _1 in Parse.String("door")
            from name in IdentParser
            from label in QuotedStringParser
            from options in DoorOptionParser.Token().Many()
            select new Door(name, label, options);

        internal static Parser<Enum.Direction> DirectionParser =
            Parse.String("north").Return(Enum.Direction.North)
            .Or(Parse.String("east").Return(Enum.Direction.East))
            .Or(Parse.String("south").Return(Enum.Direction.South))
            .Or(Parse.String("west").Return(Enum.Direction.West));

        internal static readonly Parser<Exit> ExitParser =
            from _1 in Parse.String("exit").Token()
            from dir in DirectionParser.Token()
            from _2 in Parse.String("to").Token()
            from target in IdentParser.Token()
            from door in (from _3 in Parse.String("via")
                          from door in IdentParser
                          select door).Or(Parse.Return(""))
            select new Exit(dir, target, door);

        internal static readonly Parser<Flag> FlagParser =
            from _1 in Parse.String("flag").Token()
            from name in IdentParser.Token()
            from set in (from _3 in Parse.String("is").Token()
                            from _4 in Parse.String("set").Token()
                            select true).Or(from _3 in Parse.String("is").Token()
                                             from _4 in Parse.String("not").Token()
                                             from _5 in Parse.String("set").Token()
                                             select false)
            select new Flag(name, set);

        internal static readonly Parser<ConversationItem> ConversationItemParser =
            (from _1 in Parse.String("statement").Token()
             from number in Parse.Number
             from _2 in Parse.String(":").Token()
             from section in SectionParser
             select new ConversationItem(
                 Enum.ConversationItemType.StatementDefinition,
                 int.Parse(number), 0, section, new List<int>()))
            .Or(from _1 in Parse.String("response").Token()
                from number in Parse.Number
                from _2 in Parse.String(":").Token()
                from section in SectionParser
                select new ConversationItem(
                    Enum.ConversationItemType.ResponseDefinition,
                    int.Parse(number), 0, section, new List<int>()))
            .Or(from _1 in Parse.String("statement").Token()
                from number in Parse.Number
                from _2 in Parse.String("has").Token()
                from _3 in (Parse.String("responses").Or(Parse.String("response")))
                from numbers in NumbersParser
                select new ConversationItem(
                    Enum.ConversationItemType.StatementHasResponses,
                    int.Parse(number), 0, null, numbers))
            .Or(from _1 in Parse.String("response").Token()
                from responseNumber in Parse.Number
                from _2 in Parse.String("causes").Token()
                from _3 in Parse.String("statement").Token()
                from statementNumber in Parse.Number
                select new ConversationItem(Enum.ConversationItemType.ResponseCausesStatement,
                    int.Parse(responseNumber), int.Parse(statementNumber), null, new List<int>()))
            .Or(from _1 in Parse.String("response").Token()
                from responseNumber in Parse.Number
                from _2 in Parse.String("ends").Token()
                from _3 in Parse.String("the").Token()
                from _4 in Parse.String("conversation").Token()
                select new ConversationItem(Enum.ConversationItemType.ResponseEndsConversation,
                    int.Parse(responseNumber), 0, null, new List<int>()));

        internal static readonly Parser<Conversation> ConversationParser =
            from _1 in Parse.String("conversation").Token()
            from name in IdentParser
            from _2 in DashesParser
            from items in ConversationItemParser.Many()
            select new Conversation(name, items);

        internal static readonly Parser<CharacterOption> CharacterOptionParser =
            (from _1 in Parse.String("starts").Token()
             from _2 in Parse.String("at").Or(Parse.String("in")).Token()
             from loc in IdentParser
             from _3 in Parse.String(",").Optional()
             select new CharacterOption(Enum.CharacterOption.StartsAt, loc))
            .Or(from _1 in Parse.String("hidden").Token()
                from _2 in Parse.String(",").Optional()
                select new CharacterOption(Enum.CharacterOption.Hidden))
            .Or(from _1 in Parse.String("follows").Token()
                from _2 in Parse.String("player").Token()
                from num in Parse.Number.Token()
                from _3 in Parse.String("steps").Or(Parse.String("step")).Token()
                from _4 in Parse.String("behind")
                from _5 in Parse.String(",").Optional()
                select new CharacterOption(Enum.CharacterOption.FollowsPlayerBehind, int.Parse(num)))
            .Or(from _1 in Parse.String("conversation").Token()
                from _2 in Parse.String("is").Token()
                from _3 in Parse.String(",").Optional()
                from conv in IdentParser
                select new CharacterOption(Enum.CharacterOption.ConversationIs, conv));

        internal static readonly Parser<Character> CharacterParser =
            from _1 in Parse.String("character").Token()
            from name in IdentParser
            from label in QuotedStringParser
            from options in CharacterOptionParser.Many().Token()
            from _2 in DashesParser
            from section in SectionParser
            from conversations in ConversationParser.Many()
            select new Character(name, label, options, section, conversations);

        internal static readonly Parser<IElement> LocationParser =
            from _1 in Parse.String("location").Token()
            from name in IdentParser
            from label in QuotedStringParser
            from _2 in DashesParser
            from section in SectionParser
            from flags in FlagParser.Many().Token()
            from items in ItemParser.Many().Token()
            from doors in DoorParser.Many().Token()
            from exits in ExitParser.Many().Token()
            select new Location(name, label, section, flags, items, doors, exits);

        internal static readonly Parser<IElement> DocumentParser =
            LocationParser.Or(CharacterParser).End();
    }
}
