using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Location : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter, ISemanticsValidator
    {
        public string Name { get; }
        public string Label { get; }
        public Section Section { get; }
        public IEnumerable<Flag> Flags { get; }
        public IEnumerable<Item> Items { get; }
        public IEnumerable<Door> Doors { get; }
        public IEnumerable<Exit> Exits { get; }

        public Location(string name, string label, Section section, IEnumerable<Flag> flags, IEnumerable<Item> items,
            IEnumerable<Door> doors, IEnumerable<Exit> exits)
        {
            Name = name;
            Label = label;
            Section = section;
            Flags = flags;
            Items = items;
            Doors = doors;
            Exits = exits;
        }

        public void CheckSemantics(ISemantics semantics)
        {
            foreach (var item in Items)
            {
                semantics.AddItem(item);
                item.CheckSemantics(semantics);
            }
            foreach (var flag in Flags)
            {
                semantics.AddFlag(flag);
            }
            foreach (var door in Doors)
            {
                semantics.AddDoor(door);
            }
            foreach (var exit in Exits)
            {
                semantics.AddExit(exit);
            }
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var header = $"location {Name} \"{Label}\"".CreateHeader();
            var section = Section.EmitTheseusCode(indent).AppendNewLine();
            var flags = Flags.EmitSourceCode(indent + 2).PrependNewLineIfNotEmpty();
            var items = Items.EmitSourceCode(indent + 2);
            var doors = Doors.EmitSourceCode(indent + 2).PrependNewLineIfNotEmpty();
            var exits = Exits.EmitSourceCode(indent + 2).PrependNewLineIfNotEmpty();
            return $"{header}{section}{flags}{items}{doors}{exits}";
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            var doors = Doors.EmitJavaScript(Environment.NewLine).AppendNewLineIfNotEmpty();
            var items = Items.EmitJavaScript(Environment.NewLine).PrependAndAppendNewLineIfNotEmpty();

            var loc = $"var {Name} = (function() {{".Indent(indent).AppendNewLine();
            loc += "return {".Indent(indent + 4).AppendNewLine();
            loc += $"name: \"{Name}\",".Indent(indent + 8).AppendNewLine();
            loc += $"caption: \"{Label}\",".Indent(indent + 8).AppendNewLine();
            loc += $"items: \"{string.Join(",", GetItems(false).Concat(GetDoors()))}\",".Indent(indent + 8).AppendNewLine();
            loc += "getExits: getExits,".Indent(indent + 8).AppendNewLine();
            loc += "look: look,".Indent(indent + 8).AppendNewLine();
            loc += "}".Indent(indent + 4).AppendNewLine().AppendNewLine();

            var getExits = "function getExits() {".Indent(indent + 4).AppendNewLine();
            getExits += "var _exits = { };".Indent(indent + 8).AppendNewLine();
            getExits += Exits.EmitJavaScript(indent + 8, Environment.NewLine).AppendNewLineIfNotEmpty();
            getExits += "return _exits;".Indent(indent + 8).AppendNewLine();
            getExits += "}".Indent(indent + 4).AppendNewLine().AppendNewLine();
            loc += getExits;

            var look = "function look() {".Indent(indent + 4).AppendNewLine();
            look += "_s = \"\";".Indent(indent + 8).AppendNewLine();
            look += Section.EmitJavaScriptCode(indent + 8);
            look += "return _s;".Indent(indent + 8).AppendNewLine();
            look += "}".Indent(indent + 4).AppendNewLine();
            loc += look;

            loc += "}".Indent(indent).AppendNewLine();

            return $"{doors}{items}{loc}";
        }

        private IEnumerable<string> GetItems(bool includeHidden)
        {
            return Items.Where(i => includeHidden || !i.Hidden).Select(i => i.Name);
        }

        private IEnumerable<string> GetDoors()
        {
            return Doors.Select(d => d.Name);
        }
    }
}
