using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Conversation : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
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

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            cb.Add($"function {Name}() {{").In();
            ConversationItems.EmitJavaScript(semantics, cb);
            cb.Add("conversation.startConversation(1);").Out();
            cb.Add("}");
        }
    }

}
