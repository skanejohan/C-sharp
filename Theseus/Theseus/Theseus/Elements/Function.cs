﻿using System;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Function : IElement, IComparable, ITheseusCodeEmitter, IJavaScriptCodeEmitter
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

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherItem = obj as Function;
            if (otherItem == null)
            {
                return 1;
            }

            return Name.CompareTo(otherItem.Name);
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var hidden = Hidden ? " hidden" : "";
            var header = $"function {Name} \"{Label}\"{hidden}".CreateHeader(indent);
            var section = Section.EmitTheseusCode(indent).AppendNewLine();
            return $"{header}{section}".PrependNewLine();
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            var visible = Hidden ? "false" : "true";
            cb.Add($"function {Name}(context) {{").In();
            cb.Add("_s = \"\";");
            Section.EmitJavaScriptCode(semantics, cb);
            cb.Add("context.setMessage(_s);").Out();
            cb.Add("}");
            cb.Add($"var {Name}_visible = {visible};");
            cb.Add($"{Name}.isVisible = () => {Name}_visible;");
            cb.Add($"{Name}.setVisible = value => {Name}_visible = value;");
            cb.Add($"additionalVerbs.add(\"{Label}\", {Name});");
            cb.Add();
        }
    }
}
