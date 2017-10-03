using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Enumerations;
using Theseus.Interfaces;
using Theseus.Extensions;

namespace Theseus.Elements
{
    // TODO support semantics check - responses and statements should match!
    public class ConversationItem : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public ConversationItemType Type { get; }
        public int Number { get; }
        public int CausesNumber { get; }
        public Section Section { get; }
        public IEnumerable<int> Responses;

        public ConversationItem(ConversationItemType type, int number, int causesNumber, Section section, IEnumerable<int> responses)
        {
            Type = type;
            Number = number;
            CausesNumber = causesNumber;
            Section = section;
            Responses = responses?.ToList();
        }

        public string EmitTheseusCode(int indent = 0)
        {
            switch (Type)
            {
                case ConversationItemType.StatementDefinition:
                    return $"statement {Number}: {Section.EmitTheseusCode()}".Indent(indent).AppendNewLine();
                case ConversationItemType.ResponseDefinition:
                    return $"response {Number}: {Section.EmitTheseusCode()}".Indent(indent).AppendNewLine();
                case ConversationItemType.StatementHasResponses:
                    var resp = Responses.Count() == 1 ? "response" : "responses";
                    return $"statement {Number} has {resp} {string.Join(", ", Responses)}".Indent(indent).AppendNewLine();
                case ConversationItemType.ResponseCausesStatement:
                    return $"response {Number} causes statement {CausesNumber}".Indent(indent).AppendNewLine();
                default:
                    return $"response {Number} ends the conversation".Indent(indent).AppendNewLine();
            }
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            switch (Type)
            {
                case ConversationItemType.StatementDefinition:
                case ConversationItemType.ResponseDefinition:
                    return $"conversation.addStatement({Number}, \"{Section.EmitTheseusCode()}\");".Indent(indent);
                case ConversationItemType.StatementHasResponses:
                    return $"conversation.setResponses({Number}, [{string.Join(", ", Responses)}]);".Indent(indent);
                case ConversationItemType.ResponseCausesStatement:
                    return $"conversation.setResponses({Number}, [{CausesNumber}]);".Indent(indent);
                default:
                    return $"conversation.setResponses({Number}, [0]);".Indent(indent);
            }
        }
    }
}
