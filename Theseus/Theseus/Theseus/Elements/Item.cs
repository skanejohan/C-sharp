using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Item : IElement, IComparable, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public int Level { get; }
        public string Name { get; }
        public string Label { get; }
        public bool Hidden { get; }
        public IEnumerable<ItemOption> Options { get; }
        public Section Section { get; }
        public IEnumerable<Function> Functions { get; }
        public IEnumerable<ItemAction> Actions { get; }

        public Item(int level, string name, string label, bool hidden, IEnumerable<ItemOption> options,
            Section section, IEnumerable<Function> functions, IEnumerable<ItemAction> actions)
        {
            Level = level;
            Name = name;
            Label = label;
            Hidden = hidden;
            Options = options.ToList();
            Section = section;
            Functions = functions.ToList();
            Actions = actions.ToList();
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherItem = obj as Item;
            if (otherItem == null)
            {
                return 1;
            }

            return Name.CompareTo(otherItem.Name);
        }

        public void CheckSemantics(ISemantics semantics)
        {
            foreach (var function in Functions)
            {
                semantics.AddFunction(function);
            }
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var actualIndent = indent + Level * 2;
            var pluses = new string('+', Level).AppendIfNotEmpty(" ");
            var hidden = Hidden ? " hidden" : "";
            var options = Options.EmitSourceCode(" ").PrependIfNotEmpty(" ");
            var header = $"{pluses}item {Name} \"{Label}\"{hidden}{options}".CreateHeader(actualIndent).PrependNewLine();
            var section = Section.EmitTheseusCode(actualIndent).AppendNewLine();
            var functions = Functions.EmitSourceCode(actualIndent + 2);
            var actions = Actions.EmitSourceCode(actualIndent + 2, Environment.NewLine).AppendNewLineIfNotEmpty();

            return $"{header}{section}{functions}{actions}";
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            var obj = "var obj = {".Indent(indent + 4).PrependAndAppendNewLineIfNotEmpty();
            obj += $"name: \"{Name}\",".Indent(indent + 8).AppendNewLine();
            obj += $"caption: \"{Label}\",".Indent(indent + 8).AppendNewLine();
            obj += $"verbs: [{string.Join(",", GetVerbs(false))}]".Indent(indent + 8).AppendNewLine();
            foreach (var verb in GetVerbs(true))
            {
                obj += $"{verb}: {verb}".Indent(indent + 8).AppendNewLine();
            }
            obj += "}".Indent(indent + 4).AppendNewLine().AppendNewLine();

            var examine = "function examine() {".Indent(indent + 4).AppendNewLine();
            examine += "_s = \"\";".Indent(indent + 8).AppendNewLine();
            examine += Section.EmitJavaScriptCode(indent + 8);
            examine += "game.message = _s;".Indent(indent + 8).AppendNewLine();
            examine += "}".Indent(indent + 4).AppendNewLine();

            var funs = Functions.EmitJavaScript(indent + 4, Environment.NewLine).AppendNewLineIfNotEmpty();

            return $"var {Name} = (function() {{{obj}{examine}{funs}}}".AppendNewLine();
        }

        private List<string> GetVerbs(bool includeHidden)
        {
            var result = new List<string>();
            result.Add("examine");
            foreach (var fun in Functions)
            {
                if (includeHidden || !fun.Hidden)
                    result.Add(fun.Name);
            }
            return result;
        }
    }
}
