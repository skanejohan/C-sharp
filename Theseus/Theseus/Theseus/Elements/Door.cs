using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Enumerations;
using Theseus.Elements.Extensions;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Door : IElement, IComparable, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public string Name { get; }
        public string Label { get; }
        public IEnumerable<ItemOption> Options { get; }

        public Door(string name, string label, IEnumerable<ItemOption> options)
        {
            Name = name;
            Label = label;
            Options = options.ToList();
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherItem = obj as Door;
            if (otherItem == null)
            {
                return 1;
            }

            return Name.CompareTo(otherItem.Name);
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var options = Options.EmitSourceCode(" ").PrependIfNotEmpty(" ");
            return $"door {Name} \"{Label}\"{options}".Indent(indent).AppendNewLine();
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            cb.Add($"var {Name} = (function() {{").In();
            cb.Add($"var isClosed = {JS.Bool(!HasOption(ItemOptionType.Open))};");
            if (HasOption(ItemOptionType.Lockable))
            {
                cb.Add($"var isLocked = {JS.Bool(HasOption(ItemOptionType.Locked))};");
            }
            cb.Add();

            cb.Add("function getVerbs(context) {").In();
            cb.Add("verbs = new collection();");
            if (HasOption(ItemOptionType.Lockable))
            {
                cb.Add("if(isClosed && !isLocked) {").In();
            }
            else
            {
                cb.Add("if(isClosed) {").In();
            }
            cb.Add("verbs.add(\"Open\", open);").Out();
            cb.Add("}");
            cb.Add("if(!isClosed) {").In();
            cb.Add("verbs.add(\"Close\", close);").Out();
            cb.Add("}");
            if (HasOption(ItemOptionType.Lockable))
            {
                cb.Add($"if (isLocked && context.inventory().has({GetRequiredKey()})) {{").In();
                cb.Add("verbs.add(\"Unlock\", unlock);").Out();
                cb.Add("}");
                cb.Add($"if (!isLocked && context.inventory().has({GetRequiredKey()})) {{").In();
                cb.Add("verbs.add(\"Lock\", lock);").Out();
                cb.Add("}");
            }
            cb.Add("return verbs;").Out();
            cb.Add("}");
            cb.Add();

            cb.Add("function open(context) {").In();
            cb.Add($"context.setMessage(\"You open the {Label}\");");
            cb.Add("isClosed = false;").Out();
            cb.Add("}");
            cb.Add();

            cb.Add("function close(context) {").In();
            cb.Add($"context.setMessage(\"You close the {Label}\");");
            cb.Add("isClosed = true;").Out();
            cb.Add("}");
            cb.Add();

            if (HasOption(ItemOptionType.Lockable))
            {
                cb.Add("function lock(context) {").In();
                cb.Add($"context.setMessage(\"You lock the {Label}\");");
                cb.Add("isLocked = true;").Out();
                cb.Add("}");
                cb.Add();

                cb.Add("function unlock(context) {").In();
                cb.Add($"context.setMessage(\"You unlock the {Label}\");");
                cb.Add("isLocked = false;").Out();
                cb.Add("}");
                cb.Add();
            }

            cb.Add("return {").In();
            cb.Add($"caption: \"{Label}\",");
            cb.Add("getVerbs: getVerbs,");
            cb.Add("isVisible: () => true,");
            cb.Add("isClosed: () => isClosed,");
            if (HasOption(ItemOptionType.Lockable))
            {
                cb.Add("isLocked: () => isLocked,");
            }
            cb.Out();
            cb.Add("}");
            cb.Add();

            cb.Out();
            cb.Add("})();");
            cb.Add();
        }

        private ItemOption GetOption(ItemOptionType type)
        {
            return Options.FirstOrDefault(io => io.Type == type);
        }

        private bool HasOption(ItemOptionType type)
        {
            return Options.Any(io => io.Type == type);
        }

        private string GetRequiredKey()
        {
            return Options.FirstOrDefault(i => i.Type == Enumerations.ItemOptionType.RequiresKey).Data;
        }
    }
}
