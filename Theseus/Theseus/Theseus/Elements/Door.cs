using System;
using System.Collections.Generic;
using Theseus.Elements.Extensions;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Door : BaseItem, IElement, IComparable, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public Door(string name, string label, IEnumerable<ItemOption> options) :
            base(name, label, false, options)
        {
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
            var gName = $"THESEUS.{GameUtils.GameName.ToUpper()}";

            cb.Add($"{gName}.{Name} = new THESEUS.Item({{").In();
            cb.Add($"caption: \"{Label}\",");
            cb.Add(IsOpenable() || IsLockable(), $"isOpenable: true,");
            cb.Add(IsClosed(), $"isClosed: true,");
            cb.Add(IsLockable(), $"isLockable: true,");
            cb.Add(IsPickable(), $"isPickable: true,");
            cb.Add(IsLocked(), $"isLocked: true,");
            cb.Add(RequiresKey(), $"requiredKey: {gName}.{GetRequiredKey()},");
            cb.Add(RequiresCombination(), $"requiredCombination: \"{GetRequiredCombination()}\",");
            cb.Add("isFixed: true,");

            cb.Out();
            cb.Add("});");
            return;
        }
    }
}