using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Flag : IElement, ITheseusCodeEmitter
    {
        public string Name { get; }
        public bool Set { get; }

        public Flag(string name, bool set)
        {
            Name = name;
            Set = set;
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
