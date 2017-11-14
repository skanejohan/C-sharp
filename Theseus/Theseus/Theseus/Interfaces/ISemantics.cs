using System;
using System.Collections.Generic;
using Theseus.Elements;
using Theseus.EventArgs;

namespace Theseus.Interfaces
{
    public interface ISemantics
    {
        IEnumerable<Item> Items { get; }
        IEnumerable<Flag> Flags { get; }
        IEnumerable<Door> Doors { get; }
        IEnumerable<Exit> Exits { get; }
        IEnumerable<Function> Functions { get; }
        IEnumerable<Location> Locations { get; }
        IEnumerable<Character> Characters { get; }
        IEnumerable<Conversation> Conversations { get; }

        event EventHandler<DuplicateEventArgs<Item>> ItemAlreadyExists;
        event EventHandler<DuplicateEventArgs<Flag>> FlagAlreadyExists;
        event EventHandler<DuplicateEventArgs<Door>> DoorAlreadyExists;
        event EventHandler<DuplicateEventArgs<Exit>> ExitAlreadyExists;
        event EventHandler<DuplicateEventArgs<Function>> FunctionAlreadyExists;
        event EventHandler<DuplicateEventArgs<Location>> LocationAlreadyExists;
        event EventHandler<DuplicateEventArgs<Character>> CharacterAlreadyExists;
        event EventHandler<DuplicateEventArgs<Conversation>> ConversationAlreadyExists;

        void AddItem(Item item);
        void AddFlag(Flag flag);
        void AddDoor(Door door);
        void AddExit(Exit exit);
        void AddFunction(Function function);
        void AddLocation(Location location);
        void AddCharacter(Character character);
        void AddConversation(Conversation conversation);

        Item ItemByName(string name);
        Flag FlagByName(string name);
        Door DoorByName(string name);
        Function FunctionByName(string name);
        Location LocationByName(string name);
        Character CharacterByName(string name);
        Conversation ConversationByName(string name);

        bool HasItemByName(string name);
        bool HasFlagByName(string name);
        bool HasDoorByName(string name);
        bool HasFunctionByName(string name);
        bool HasLocationByName(string name);
        bool HasCharacterByName(string name);
        bool HasConversationByName(string name);
    }
}
