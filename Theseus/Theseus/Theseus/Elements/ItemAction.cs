using Theseus.Elements.Enumerations;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class ItemAction : IElement, ITheseusCodeEmitter
    {
        public ItemActionType Type { get; }
        public Section Section { get; }

        public ItemAction(ItemActionType type, Section section)
        {
            Type = type;
            Section = section;
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var fun = Type.ToString().ToCamelCase();
            var sec = Section.EmitTheseusCode(0);
            return $"{fun} = {sec}".Indent(indent).PrependNewLineIfNotEmpty();
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            return ""; // TODO REMOVE
        }
    }
}
