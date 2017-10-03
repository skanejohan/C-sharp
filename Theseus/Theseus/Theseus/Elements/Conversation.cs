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

        public string EmitJavaScriptCode(int indent = 0)
        {
            var s = $"function {Name}() {{".Indent(indent).AppendNewLine();
            s += ConversationItems.EmitJavaScript(indent + 4, Environment.NewLine).AppendNewLine();
            s += "conversation.startConversation(1);".Indent(indent + 4).AppendNewLine();
            s += "}".Indent(indent).AppendNewLine();
            return s;
        }
    }

}
