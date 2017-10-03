using Theseus.Elements.Enumerations;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Exit : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
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

        public string EmitTheseusCode(int indent = 0)
        {
            var door = Door == "" ? "" : $" via {Door}";
            return $"exit {Direction.ToString().ToLower()} to {Target}{door}".Indent(indent).AppendNewLine();
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            if (Door == "")
            {
                return $"_exits.{Direction} = {Target};".Indent(indent);
            }
            else
            {
                var s = $"if ({Door}.mode == DoorMode.Open) {{".Indent(indent).AppendNewLine();
                s += $"_exits.{Direction} = {Target};".Indent(indent + 4).AppendNewLine();
                s += "}".Indent(indent);
                return s;
            }
        }
    }
}
