using System.Collections.Generic;
using Theseus.Elements;

namespace Theseus.Interfaces
{
    public interface ISemantics
    {
        IEnumerable<Item> Items { get; }
        IEnumerable<Door> Doors { get; }
        IEnumerable<Flag> Flags { get; }
        IEnumerable<Function> Functions { get; }

        //TODO public event EventHandler DoorAlreadyExists... etc.

        void AddItem(Item item);
        void AddDoor(Door door);
        void AddFlag(Flag flag);
        void AddFunction(Function function);

        bool HasItemByName(string name);
        bool HasDoorByName(string name);
        bool HasFlagByName(string name);
        bool HasFunctionByName(string name);

        Item ItemByName(string name);
        Door DoorByName(string name);
        Flag FlagByName(string name);
        Function FunctionByName(string name);
    }
}
