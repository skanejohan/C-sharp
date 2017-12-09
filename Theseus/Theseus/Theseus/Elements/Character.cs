using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Character : IElement, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
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

        public void BuildSemantics(ISemantics semantics)
        {
            semantics.AddCharacter(this);
            foreach(var c in Conversations)
            {
                c.BuildSemantics(semantics);
            }
        }

        public void CheckSemantics(ISemantics semantics)
        {
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var header = $"character {Name} \"{Label}\" {CharacterOptions.EmitSourceCode(",")}".CreateHeader();
            var section = Section.EmitTheseusCode(indent).AppendNewLine();
            var convs = Conversations.EmitSourceCode(indent + 2).PrependNewLineIfNotEmpty();
            return $"{header}{section}{convs}";
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            var gName = $"THESEUS.{GameUtils.GameName.ToUpper()}";
            var location = getLocation();
            var conversation = getConversation();

            cb.Add($"{gName}.{Name} = new THESEUS.Character({{").In();
            cb.Add($"caption: \"{Label}\",");
            cb.Add(true, $"isVisible: false,"); // TODO add visibility
            cb.Add(location != "null", $"location: {location};");
            cb.Add(conversation != "", $"conversation: {gName}.{conversation},");
            cb.Add("examine: function() {").In();
            cb.Add("var _s = \"\";");
            Section.EmitJavaScriptCode(semantics, cb);
            cb.Add("context.setMessage(_s);").Out();
            cb.Add("}").Out();
            cb.Add("});");
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
