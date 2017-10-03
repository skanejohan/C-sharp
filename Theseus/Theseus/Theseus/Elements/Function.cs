using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Function : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public string Name { get; }
        public string Label { get; }
        public bool Hidden { get; }
        public Section Section { get; }

        public Function(string name, string label, bool hidden, Section section)
        {
            Name = name;
            Label = label;
            Hidden = hidden;
            Section = section;
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var hidden = Hidden ? " hidden" : "";
            var header = $"function {Name} \"{Label}\"{hidden}".CreateHeader(indent);
            var section = Section.EmitTheseusCode(indent).AppendNewLine();
            return $"{header}{section}".PrependNewLine();
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            var header = $"function {Name}() {{".Indent(indent).PrependAndAppendNewLineIfNotEmpty();
            var section = "_s = \"\";".Indent(indent + 4).AppendNewLine();
            section += Section.EmitJavaScriptCode(indent + 4);
            section += "game.message = _s;".Indent(indent + 4).AppendNewLine();
            var footer = "}".Indent(indent);
            return $"{header}{section}{footer}";
        }
    }
}
