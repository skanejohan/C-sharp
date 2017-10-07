using System;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Flag : IElement, IComparable, ITheseusCodeEmitter
    {
        public string Name { get; }
        public bool Set { get; }

        public Flag(string name, bool set)
        {
            Name = name;
            Set = set;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherItem = obj as Flag;
            if (otherItem == null)
            {
                return 1;
            }

            return Name.CompareTo(otherItem.Name);
        }

        public string EmitTheseusCode(int indent = 0)
        {
            return (Set ? $"flag {Name} is set" : $"flag {Name} is not set").Indent(indent).AppendNewLine();
        }

        public string EmitJavaScriptCode(int indent = 0) // TODO REMOVE
        {
            return ""; // Not used - code is emitted by other classes.
        }
    }
}
