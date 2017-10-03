using Theseus.Elements.Enumerations;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class ItemOption : IElement, ITheseusCodeEmitter
    {
        public ItemOptionType Type { get; }
        public string Data { get; }

        public ItemOption(ItemOptionType type, string data = "")
        {
            Type = type;
            Data = data;
        }

        public string EmitTheseusCode(int indent = 0)
        {
            switch (Type)
            {
                case ItemOptionType.RequiresCombination:
                    return "requires combination " + Data;
                case ItemOptionType.RequiresKey:
                    return "requires key " + Data;
                default:
                    return Type.ToString().ToCamelCase();
            }
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            return ""; // TODO REMOVE
        }
    }
}
