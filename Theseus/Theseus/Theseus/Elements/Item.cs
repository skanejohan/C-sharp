using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Elements.Enumerations;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Item : BaseItem, IElement, IComparable, ISemanticsValidator, IOrderable, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public int Level { get; }
        public Section Section { get; }
        public IEnumerable<Function> Functions { get; }
        public IEnumerable<ItemAction> Actions { get; }
        public Item Container { get; private set; }
        public IEnumerable<Item> Contained => containedItems;

        public int Order { get; set; }

        public Item(int level, string name, string label, bool hidden, IEnumerable<ItemOption> options,
            Section section, IEnumerable<Function> functions, IEnumerable<ItemAction> actions) :
            base(name, label, hidden, options)
        {
            Level = level;
            Section = section != null ? section : new Section(new List<IElement>());
            Functions = functions != null ? functions.ToList() : new List<Function>();
            Actions = actions != null ? actions.ToList() : new List<ItemAction>();
            containedItems = new List<Item>();
        }

        public void PutIntoThis(Item contained)
        {
            containedItems.Add(contained);
            contained.Container = this;
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

        public IEnumerable<IOrderable> GetDependencies()
        {
            foreach(var i in containedItems)
            {
                yield return i;
            }
        }

        public void BuildSemantics(ISemantics semantics)
        {
            foreach (var function in Functions)
            {
                semantics.AddFunction(function);
            }
        }

        public void CheckSemantics(ISemantics semantics)
        {
            Section.CheckSemantics(semantics);
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

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            var gName = $"THESEUS.{GameUtils.GameName.ToUpper()}";
            var hasItems = containedItems.Count > 0;

            cb.Add($"{gName}.{Name} = new THESEUS.Item({{").In();
            cb.Add($"caption: \"{Label}\",");
            cb.Add(Hidden, $"isVisible: false,");
            cb.Add(IsOpenable(), $"isOpenable: true,");
            cb.Add(IsClosed(), $"isClosed: true,");
            cb.Add(IsLockable(), $"isLockable: true,");
            cb.Add(IsPickable(), $"isPickable: true,");
            cb.Add(IsLocked(), $"isLocked: true,");
            cb.Add(RequiresKey(), $"requiredKey: {gName}.{GetRequiredKey()},");
            cb.Add(RequiresCombination(), $"requiredCombination: \"{GetRequiredCombination()}\",");
            cb.Add(IsFixed(), $"isFixed: true,");

            cb.Add(hasItems, $"containedItems: [{string.Join(", ", containedItems.Select(i => $"{gName}.{i.Name}"))}],");

            cb.Add("examine: function(context) {").In();
            cb.Add("var _s = \"\";");
            Section.EmitJavaScriptCode(semantics, cb);
            cb.Add("context.setMessage(_s);").Out();
            cb.Add("},");

            ProcessAfterAction(ItemActionType.AfterDrop, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterDropOnce, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterTake, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterTakeOnce, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterClose, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterCloseOnce, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterOpen, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterOpenOnce, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterLock, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterLockOnce, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterUnlock, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterUnlockOnce, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterPick, semantics, cb);
            ProcessAfterAction(ItemActionType.AfterPickOnce, semantics, cb);

            Functions.EmitJavaScript(semantics, cb);

            if (Functions.Count() > 0)
            {
                cb.Add("getAdditionalVerbs: function(verbs) {").In();
                foreach (var f in Functions)
                {
                    cb.Add($"if ({gName}.{this.Name}.functionIsVisible(\"{f.Name}\")) {{").In();
                    cb.Add($"verbs.add(\"{f.Label}\", this.{f.Name});").Out();
                    cb.Add("}");
                }
                cb.Out();
                cb.Add("},");
            }

            cb.Out();
            cb.Add("});");
            foreach (var f in Functions)
            {
                cb.Add($"{gName}.{this.Name}.setFunctionVisible(\"{f.Name}\", {JS.Bool(!f.Hidden)});");
            }
        }

        private List<Item> containedItems;

        private void ProcessAfterAction(ItemActionType actionType, ISemantics semantics, ICodeBuilder cb)
        {
            var action = Actions.FirstOrDefault(i => i.Type == actionType);
            if (action != null)
            {
                cb.Add($"{actionType.ToString().ToCamelCase()}: function(context) {{").In();
                cb.Add("var _s = \"\"");
                action.Section.EmitJavaScriptCode(semantics, cb);
                cb.Add("if (_s != \"\") {").In();
                cb.Add("context.setMessage(_s);").Out();
                cb.Add("}").Out();
                cb.Add("},");
            }
        }
    }
}
