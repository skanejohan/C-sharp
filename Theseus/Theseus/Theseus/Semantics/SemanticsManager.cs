using System.Collections.Generic;
using System.Linq;
using Theseus.Elements;
using Theseus.Interfaces;

namespace Theseus.Parser.Semantics
{
    // TODO: make sure that Door has no invalid item options.
    class SemanticsManager : ISemantics
    {
        public IEnumerable<Item> Items { get; }
        public IEnumerable<Door> Doors { get; }
        public IEnumerable<Flag> Flags { get; }
        public IEnumerable<Function> Functions { get; }

        public SemanticsManager()
        {
            Items = new List<Item>();
            Doors = new List<Door>();
            Flags = new List<Flag>();
            Functions = new List<Function>();
        }

        public void AddItem(Item item)
        {
        }

        public void AddDoor(Door door)
        {
        }

        public void AddFlag(Flag flag)
        {
        }

        public void AddFunction(Function function)
        {
        }

        public bool HasItemByName(string name)
        {
            return Items.Any(i => i.Name == name);
        }

        public bool HasDoorByName(string name)
        {
            return Doors.Any(i => i.Name == name);
        }

        public bool HasFlagByName(string name)
        {
            return Flags.Any(i => i.Name == name);
        }

        public bool HasFunctionByName(string name)
        {
            return Functions.Any(i => i.Name == name);
        }

        public Item ItemByName(string name)
        {
            return Items.FirstOrDefault(i => i.Name == name);
        }

        public Door DoorByName(string name)
        {
            return Doors.FirstOrDefault(i => i.Name == name);
        }

        public Flag FlagByName(string name)
        {
            return Flags.FirstOrDefault(i => i.Name == name);
        }

        public Function FunctionByName(string name)
        {
            return Functions.FirstOrDefault(i => i.Name == name);
        }

        public void Analyze(IEnumerable<IElement> documents)
        {
            // First pass - create lists of all items, doors, flags and functions
        }


    }
}
