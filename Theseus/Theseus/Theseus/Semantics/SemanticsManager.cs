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
        public IEnumerable<Location> Locations => locations;
        public IEnumerable<Character> Characters => characters;
        public IEnumerable<Conversation> Conversations => conversations;

        public SemanticsManager()
        {
            items = new SortedSet<Item>();
            flags = new SortedSet<Flag>();
            doors = new SortedSet<Door>();
            exits = new SortedSet<Exit>();
            functions = new SortedSet<Function>();
            locations = new SortedSet<Location>();
            characters = new SortedSet<Character>();
            conversations = new SortedSet<Conversation>();
        }

        public event EventHandler<DuplicateEventArgs<Item>> ItemAlreadyExists;
        public event EventHandler<DuplicateEventArgs<Flag>> FlagAlreadyExists;
        public event EventHandler<DuplicateEventArgs<Door>> DoorAlreadyExists;
        public event EventHandler<DuplicateEventArgs<Exit>> ExitAlreadyExists;
        public event EventHandler<DuplicateEventArgs<Function>> FunctionAlreadyExists;
        public event EventHandler<DuplicateEventArgs<Location>> LocationAlreadyExists;
        public event EventHandler<DuplicateEventArgs<Character>> CharacterAlreadyExists;
        public event EventHandler<DuplicateEventArgs<Conversation>> ConversationAlreadyExists;

        public void AddItem(Item item)
        {
            if (HasItemByName(item.Name))
            {
                ItemAlreadyExists?.Invoke(this, new DuplicateEventArgs<Item>() { Object = item });
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
                FlagAlreadyExists?.Invoke(this, new DuplicateEventArgs<Flag>() { Object = flag });
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
                DoorAlreadyExists?.Invoke(this, new DuplicateEventArgs<Door>() { Object = door });
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
                ExitAlreadyExists?.Invoke(this, new DuplicateEventArgs<Exit>() { Object = exit });
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
                FunctionAlreadyExists?.Invoke(this, new DuplicateEventArgs<Function>() { Object = function });
            }
            else
            {
                functions.Add(function);
            }
        }

        public void AddLocation(Location location)
        {
            if (HasLocationByName(location.Name))
            {
                LocationAlreadyExists?.Invoke(this, new DuplicateEventArgs<Location>() { Object = location });
            }
            else
            {
                locations.Add(location);
            }
        }

        public void AddCharacter(Character character)
        {
            if (HasCharacterByName(character.Name))
            {
                CharacterAlreadyExists?.Invoke(this, new DuplicateEventArgs<Character>() { Object = character });
            }
            else
            {
                characters.Add(character);
            }
        }

        public void AddConversation(Conversation conversation)
        {
            if (HasConversationByName(conversation.Name))
            {
                ConversationAlreadyExists?.Invoke(this, new DuplicateEventArgs<Conversation>() { Object = conversation });
            }
            else
            {
                conversations.Add(conversation);
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

        public Location LocationByName(string name)
        {
            return Locations.FirstOrDefault(l => l.Name == name);
        }

        public Character CharacterByName(string name)
        {
            return Characters.FirstOrDefault(c => c.Name == name);
        }

        public Conversation ConversationByName(string name)
        {
            return Conversations.FirstOrDefault(c => c.Name == name);
        }

        public void Analyze(IEnumerable<ISemanticsValidator> validators)
        {
            // First pass - create lists of all items, doors, flags and functions
            foreach (var validator in validators)
            {
                validator.BuildSemantics(this);
            }

            // TODO second pass?
        }

        public bool HasItemByName(string name)
        {
            return Items.Any(i => i.Name == name);
        }

        public bool HasFlagByName(string name)
        {
            return Flags.Any(i => i.Name == name);
        }

        public bool HasDoorByName(string name)
        {
            return Doors.Any(i => i.Name == name);
        }

        public bool HasFunctionByName(string name)
        {
            return Functions.Any(i => i.Name == name);
        }

        public bool HasLocationByName(string name)
        {
            return Locations.Any(i => i.Name == name);
        }

        public bool HasCharacterByName(string name)
        {
            return Characters.Any(c => c.Name == name);
        }

        public bool HasConversationByName(string name)
        {
            return Conversations.Any(c => c.Name == name);
        }

        private SortedSet<Item> items;
        private SortedSet<Flag> flags;
        private SortedSet<Door> doors;
        private SortedSet<Exit> exits;
        private SortedSet<Function> functions;
        private SortedSet<Location> locations;
        private SortedSet<Character> characters;
        private SortedSet<Conversation> conversations;

    }
}
