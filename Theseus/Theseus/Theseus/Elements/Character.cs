using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Character : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        // todo support semantics for conversation - we should be able to look up conversations by name, and to detect duplicates
        public string Name { get; }
        public string Label { get; }
        public IEnumerable<CharacterOption> CharacterOptions;
        public Section Section { get; }
        public IEnumerable<Conversation> Conversations { get; }

        public Character(string name, string label, IEnumerable<CharacterOption> characterOptions, Section section, IEnumerable<Conversation> conversations)
        {
            Name = name;
            Label = label;
            CharacterOptions = characterOptions.ToList();
            Section = section;
            Conversations = conversations?.ToList();
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var header = $"character {Name} \"{Label}\" {CharacterOptions.EmitSourceCode(",")}".CreateHeader();
            var section = Section.EmitTheseusCode(indent).AppendNewLine();
            var convs = Conversations.EmitSourceCode(indent + 2).PrependNewLineIfNotEmpty();
            return $"{header}{section}{convs}";
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            var s = $"var {Name} = (function() {{".Indent(indent).AppendNewLine();
            s += "return {".Indent(indent + 4).AppendNewLine();
            s += $"name: \"{Name}\",".Indent(indent + 8).AppendNewLine();
            s += $"caption: \"{Label}\",".Indent(indent + 8).AppendNewLine();
            var conv = getConversation();
            if (conv == "")
            {
                s += "verbs: [examine],".Indent(indent + 8).AppendNewLine();
            }
            else
            {
                s += "verbs: [talk, examine],".Indent(indent + 8).AppendNewLine();
                s += $"talk: {conv},".Indent(indent + 8).AppendNewLine();
            }
            s += "update: update,".Indent(indent + 8).AppendNewLine();
            s += "examine: examine,".Indent(indent + 8).AppendNewLine();
            s += $"location: {getLocation()},".Indent(indent + 8).AppendNewLine();
            s += "}".Indent(indent + 4).AppendNewLine();

            s += "function update() {".Indent(indent+4).PrependAndAppendNewLineIfNotEmpty();
            var follows = CharacterOptions.FirstOrDefault(co => co.Option == Enumerations.CharacterOption.FollowsPlayerBehind);
            if (follows != null)
            {
                s += $"game.npcs.move({Name}, game.location({follows.StepsBehind}));".Indent(indent + 8).AppendNewLine();
            }
            s += "}".Indent(indent+4).AppendNewLine();

            s += "function examine() {".Indent(indent + 4).PrependAndAppendNewLineIfNotEmpty();
            s += "_s = \"\";".Indent(indent + 8).AppendNewLine();
            s += Section.EmitJavaScriptCode(indent + 8);
            s += "game.message = _s;".Indent(indent + 8).AppendNewLine();
            s += "}".Indent(indent + 4).AppendNewLine();

            s += Conversations.EmitJavaScript(indent + 4, Environment.NewLine).PrependAndAppendNewLineIfNotEmpty();

            s += "})();".Indent(indent).AppendNewLine();

            return s;
        }

        private string getLocation()
        {
            var option = CharacterOptions.FirstOrDefault(co => co.Option == Enumerations.CharacterOption.StartsAt);
            var hidden = CharacterOptions.FirstOrDefault(co => co.Option == Enumerations.CharacterOption.Hidden);
            return option != null && hidden == null ? option.Ident : "null";
        }

        private string getConversation()
        {
            var option = CharacterOptions.FirstOrDefault(co => co.Option == Enumerations.CharacterOption.ConversationIs);
            return option == null ? "" : option.Ident;
        }
    }
}
