using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Location : IElement, IComparable, ITheseusCodeEmitter, IJavaScriptCodeEmitter, ISemanticsValidator
    {
        public string Name { get; }
        public string Label { get; }
        public Section Section { get; }
        public IEnumerable<Flag> Flags { get; }
        public IEnumerable<Item> Items { get; private set; }
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

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherItem = obj as Location;
            if (otherItem == null)
            {
                return 1;
            }

            return Name.CompareTo(otherItem.Name);
        }

        public void BuildSemantics(ISemantics semantics)
        {
            semantics.AddLocation(this);

            foreach (var item in Items)
            {
                semantics.AddItem(item);
                item.BuildSemantics(semantics);
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

            StructureItems();
        }

        private void StructureItems()
        {
            var items = Items.ToList();
            var root = new Item(-1, "", "", false, null, null, null, null);
            var current = root;

            if (items.Count > 0 && items[0].Level != 0)
            {
                throw new Exception("Invalid level");
            }

            foreach (var i in items)
            {
                if (i.Level < 0)
                {
                    throw new Exception("Invalid level");
                }

                if (i.Level == current.Level + 1)
                {
                    current.PutIntoThis(i);
                }
                else if (i.Level <= current.Level)
                {
                    while (i.Level <= current.Level)
                    {
                        current = current.Container;
                    }
                    current.PutIntoThis(i);
                }
                else
                {
                    throw new Exception("Invalid level");
                }
                current = i;
            }
            Items = root.Contained;
        }

        public void CheckSemantics(ISemantics semantics)
        {
            Section.CheckSemantics(semantics);
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

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            cb.Add($"var {Name} = (function() {{").In();

            cb.Add($"var _items = new items();");
            foreach (var i in GetItems(true))
            {
                cb.Add($"_items.add({i});");
            }
            cb.Add($"var _characters = new items();");
            foreach (var c in semantics.Characters)
            {
                foreach(var o in c.CharacterOptions)
                {
                    if (o.Option == Enumerations.CharacterOption.StartsAt &&
                        o.Ident == Name)
                    {
                        cb.Add($"_characters.add({c.Name});");
                    }
                }
            }
            foreach (var d in GetDoors())
            {
                cb.Add($"_items.add({d});");
            }
            cb.Add();

            cb.Add("return {").In();
            cb.Add($"name: \"{Name}\",");
            cb.Add($"caption: \"{Label}\",");
            cb.Add("items: _items,");
            cb.Add("characters: _characters,");
            cb.Add("getExits: getExits,");
            cb.Add("look: look,").Out();
            cb.Add("}");
            cb.Add();

            cb.Add("function getExits() {").In();
            cb.Add("var _exits = { };");
            Exits.EmitJavaScript(semantics, cb);
            cb.Add("return _exits;").Out();
            cb.Add("}");
            cb.Add();

            cb.Add("function look() {").In();
            cb.Add("_s = \"\";");
            Section.EmitJavaScriptCode(semantics, cb);
            cb.Add("return _s;").Out();
            cb.Add("}");
            cb.Add().Out();

            cb.Add("})();");
            cb.Add();
        }

        private IEnumerable<string> GetItems(bool includeHidden)
        {
            return Items.Where(i => (includeHidden || !i.Hidden)).Select(i => i.Name);
        }

        private IEnumerable<Item> GetAllItems()
        {
            foreach(var i in Items)
            {
                foreach (var ii in GetThisItem(i))
                {
                    yield return ii;
                }
            }
        }

        private IEnumerable<Item> GetThisItem(Item item)
        {
            yield return item;
            foreach (var i in item.Contained)
            {
                foreach (var ii in GetThisItem(i))
                {
                    yield return ii;
                }
            }
        }

        private IEnumerable<string> GetDoors()
        {
            return Doors.Select(d => d.Name);
        }
    }
}
