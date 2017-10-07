using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements;
using Theseus.EventArgs;
using Theseus.Interfaces;

namespace Theseus.Semantics
{
    class SemanticsManager : ISemantics
    {
        public IEnumerable<Item> Items => items;
        public IEnumerable<Flag> Flags => flags;
        public IEnumerable<Door> Doors => doors;
        public IEnumerable<Exit> Exits => exits;
        public IEnumerable<Function> Functions => functions;

        public SemanticsManager()
        {
            items = new SortedSet<Item>();
            flags = new SortedSet<Flag>();
            doors = new SortedSet<Door>();
            exits = new SortedSet<Exit>();
            functions = new SortedSet<Function>();
        }

        public event EventHandler<ItemDuplicateEventArgs> ItemAlreadyExists;
        public event EventHandler<FlagDuplicateEventArgs> FlagAlreadyExists;
        public event EventHandler<DoorDuplicateEventArgs> DoorAlreadyExists;
        public event EventHandler<ExitDuplicateEventArgs> ExitAlreadyExists;
        public event EventHandler<FunctionDuplicateEventArgs> FunctionAlreadyExists;

        public void AddItem(Item item)
        {
            if (HasItemByName(item.Name))
            {
                ItemAlreadyExists?.Invoke(this, new ItemDuplicateEventArgs() { Item = item });
            }
            else
            {
                items.Add(item);
            }
        }

        public void AddFlag(Flag flag)
        {
            if (HasFlagByName(flag.Name))
            {
                FlagAlreadyExists?.Invoke(this, new FlagDuplicateEventArgs() { Flag = flag });
            }
            else
            {
                flags.Add(flag);
            }
        }

        public void AddDoor(Door door)
        {
            if (HasDoorByName(door.Name))
            {
                DoorAlreadyExists?.Invoke(this, new DoorDuplicateEventArgs() { Door = door });
            }
            else
            {
                doors.Add(door);
            }
        }

        public void AddExit(Exit exit)
        {
            if (Exits.Any(e => e.CompareTo(exit) == 0))
            {
                ExitAlreadyExists?.Invoke(this, new ExitDuplicateEventArgs() { Exit = exit });
            }
            else
            {
                exits.Add(exit);
            }
        }

        public void AddFunction(Function function)
        {
            if (HasFunctionByName(function.Name))
            {
                FunctionAlreadyExists?.Invoke(this, new FunctionDuplicateEventArgs() { Function = function });
            }
            else
            {
                functions.Add(function);
            }
        }

        public Item ItemByName(string name)
        {
            return Items.FirstOrDefault(i => i.Name == name);
        }

        public Flag FlagByName(string name)
        {
            return Flags.FirstOrDefault(i => i.Name == name);
        }

        public Door DoorByName(string name)
        {
            return Doors.FirstOrDefault(i => i.Name == name);
        }

        public Function FunctionByName(string name)
        {
            return Functions.FirstOrDefault(i => i.Name == name);
        }

        public void Analyze(IEnumerable<ISemanticsValidator> validators)
        {
            // First pass - create lists of all items, doors, flags and functions
            foreach (var validator in validators)
            {
                validator.CheckSemantics(this);
            }
            /*
             Action
             Character
             CharacterOption
             Conversation
             ConversationItem
             Door
             Effect
             Exit
             Expressions
             Flag
             Function
             IfStatement
             Item                   Function
             ItemAction
             ItemOption
             Location             Flag, Item, Door, Exit
             Section
             SectionText
             SimpleCondition
             */
        }

        private SortedSet<Item> items;
        private SortedSet<Flag> flags;
        private SortedSet<Door> doors;
        private SortedSet<Exit> exits;
        private SortedSet<Function> functions;

        private bool HasItemByName(string name)
        {
            return Items.Any(i => i.Name == name);
        }

        private bool HasFlagByName(string name)
        {
            return Flags.Any(i => i.Name == name);
        }

        private bool HasDoorByName(string name)
        {
            return Doors.Any(i => i.Name == name);
        }

        private bool HasFunctionByName(string name)
        {
            return Functions.Any(i => i.Name == name);
        }
    }
}
