using System;
using Theseus.Elements.Enumerations;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Exit : IElement, IComparable, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public Direction Direction { get; }
        public string Target { get; }
        public string Door { get; }

        public Exit(Direction direction, string target, string door = "")
        {
            Direction = direction;
            Target = target;
            Door = door;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherExit = obj as Exit;
            if (otherExit == null)
            {
                return 1;
            }

            if (Target == otherExit.Target)
            {
                if (Direction == otherExit.Direction)
                {
                    return Door.CompareTo(otherExit.Door);
                }
                return Direction.CompareTo(otherExit.Direction);
            }
            return Target.CompareTo(otherExit.Target);
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var door = Door == "" ? "" : $" via {Door}";
            var dir = "";
            switch (Direction)
            {
                case Direction.N:
                    dir = "north";
                    break;
                case Direction.E:
                    dir = "east";
                    break;
                case Direction.S:
                    dir = "south";
                    break;
                case Direction.W:
                    dir = "west";
                    break;
            }
            return $"exit {dir} to {Target}{door}".Indent(indent).AppendNewLine();
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            if (Door == "")
            {
                cb.Add($"_exits.{Direction} = {Target};");
            }
            else
            {
                cb.Add($"if (!{Door}.isClosed()) {{").In();
                cb.Add($"_exits.{Direction} = {Target};").Out();
                cb.Add("}");
            }
        }
    }
}
