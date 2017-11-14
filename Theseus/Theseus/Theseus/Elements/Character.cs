using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
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
            cb.Add($"var {Name} = (function() {{").In();
            cb.Add($"var isVisible = false;"); // TODO add visibility
            cb.Add($"var location = {getLocation()};");

            cb.Add();
            cb.Add("function getVerbs(context) {").In();
            cb.Add("verbs = new collection();");
            cb.Add("verbs.add(\"Examine\", examine);");
            var conv = getConversation();
            if (conv != "")
            {
                cb.Add($"verbs.add(\"Talk\", {conv});");
            }
            cb.Add("return verbs;").Out();
            cb.Add("}");
            cb.Add();

            cb.Add("return {").In();
            cb.Add($"caption: \"{Label}\",");
            cb.Add("getVerbs: getVerbs,");
            cb.Add("update: update,");
            cb.Add("examine: examine,");
            cb.Add("isVisible: () => isVisible,");
            cb.Add("setVisible: value => isVisible = value,").Out();
            cb.Add("}");
            cb.Add();

            cb.Add("function update() {").In();
            var follows = CharacterOptions.FirstOrDefault(co => co.Option == Enumerations.CharacterOption.FollowsPlayerBehind);
            if (follows != null)
            {
                cb.Add("if (location != null) {").In();
                cb.Add($"location.characters.remove({Name})").Out();
                cb.Add("}");
                cb.Add($"location = context.historicLocation({follows.StepsBehind});");
                cb.Add("if (location != null) {").In();
                cb.Add($"location.characters.add({Name});").Out();
                cb.Add("}");
            }
            cb.Out();
            cb.Add("}");
            cb.Add();

            cb.Add("function examine() {").In();
            cb.Add("_s = \"\";");
            Section.EmitJavaScriptCode(semantics, cb);
            cb.Add("context.setMessage(_s);").Out();
            cb.Add("}");
            cb.Add();

            Conversations.EmitJavaScript(semantics, cb);

            cb.Out();
            cb.Add("})();");
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
