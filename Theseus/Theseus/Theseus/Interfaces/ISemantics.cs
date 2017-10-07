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

        event EventHandler<ItemDuplicateEventArgs> ItemAlreadyExists;
        event EventHandler<FlagDuplicateEventArgs> FlagAlreadyExists;
        event EventHandler<DoorDuplicateEventArgs> DoorAlreadyExists;
        event EventHandler<ExitDuplicateEventArgs> ExitAlreadyExists;
        event EventHandler<FunctionDuplicateEventArgs> FunctionAlreadyExists;

        void AddItem(Item item);
        void AddFlag(Flag flag);
        void AddDoor(Door door);
        void AddExit(Exit exit);
        void AddFunction(Function function);

        Item ItemByName(string name);
        Flag FlagByName(string name);
        Door DoorByName(string name);
        Function FunctionByName(string name);
    }
}
