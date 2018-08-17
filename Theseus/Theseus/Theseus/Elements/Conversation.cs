using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Conversation : IElement, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public string Name { get; }
        public IEnumerable<ConversationItem> ConversationItems { get; }

        public Conversation(string name, IEnumerable<ConversationItem> conversationItems)
        {
            Name = name;
            ConversationItems = conversationItems.ToList();
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var header = $"conversation {Name}".CreateHeader(indent);
            var items = ConversationItems.EmitSourceCode(indent);
            return $"{header}{items}";
        }

        public void BuildSemantics(ISemantics semantics)
        {
            semantics.AddConversation(this);
        }

        public void CheckSemantics(ISemantics semantics)
        {
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            var gName = $"THESEUS.{GameUtils.GameName.ToUpper()}";

            cb.Add($"{gName}.{Name} = function() {{").In();
            cb.Add("var conversation = new THESEUS.Conversation();");
            ConversationItems.EmitJavaScript(semantics, cb);
            cb.Add("conversation.startConversation(1);");
            cb.Add("return conversation;").Out();
            cb.Add("}");
        }
    }

}
