using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Enumerations;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Door : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public string Name { get; }
        public string Label { get; }
        public IEnumerable<ItemOption> Options { get; }

        public Door(string name, string label, IEnumerable<ItemOption> options)
        {
            Name = name;
            Label = label;
            Options = options.ToList();
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var options = Options.EmitSourceCode(" ").PrependIfNotEmpty(" ");
            return $"door {Name} \"{Label}\"{options}".Indent(indent).AppendNewLine();
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            // TODO support further option types
            string doorMode = "DoorMode.Open";
            if (HasOption(ItemOptionType.Closed))
            {
                doorMode = "DoorMode.Closed";
            }
            if (HasOption(ItemOptionType.Locked))
            {
                doorMode = "DoorMode.Locked";
            }

            string key = "";
            if (HasOption(ItemOptionType.RequiresKey))
            {
                key = GetOption(ItemOptionType.RequiresKey).Data;
            }

            if (HasOption(ItemOptionType.Lockable))
            {
                return $"var {Name} = lockableDoor(\"{Name}\", \"{Label}\", {doorMode}, {key});";

            }
            return "";
        }

        private ItemOption GetOption(ItemOptionType type)
        {
            return Options.FirstOrDefault(io => io.Type == type);
        }

        private bool HasOption(ItemOptionType type)
        {
            return Options.Any(io => io.Type == type);
        }
    }
}
