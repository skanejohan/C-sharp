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
    public class Item : IElement, IComparable, ISemanticsValidator, IOrderable, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public int Level { get; }
        public string Name { get; }
        public string Label { get; }
        public bool Hidden { get; }
        public IEnumerable<ItemOption> Options { get; }
        public Section Section { get; }
        public IEnumerable<Function> Functions { get; }
        public IEnumerable<ItemAction> Actions { get; }
        public Item Container { get; private set; }
        public IEnumerable<Item> Contained => containedItems;

        public int Order { get; set; }

        public Item(int level, string name, string label, bool hidden, IEnumerable<ItemOption> options,
            Section section, IEnumerable<Function> functions, IEnumerable<ItemAction> actions)
        {
            Level = level;
            Name = name;
            Label = label;
            Hidden = hidden;
            Options = options?.ToList();
            Section = section;
            Functions = functions?.ToList();
            Actions = actions?.ToList();
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
            cb.Add($"var {Name} = (function() {{").In();

            // Variable declarations
            cb.Add($"var isVisible = {JS.Bool(!Hidden)};");
            if (HasOptionType(ItemOptionType.Openable) || 
                HasOptionType(ItemOptionType.Lockable) ||
                HasOptionType(ItemOptionType.Pickable))
            {
                var closed = HasOptionType(ItemOptionType.Closed) || HasOptionType(ItemOptionType.Locked);
                cb.Add($"var isOpen = {JS.Bool(!closed)};");
                cb.Add($"var hasBeenOpen = {JS.Bool(!closed)};");
                cb.Add($"var isClosed = {JS.Bool(closed)};");
                cb.Add($"var hasBeenClosed = {JS.Bool(closed)};");
            }
            if (HasOptionType(ItemOptionType.Lockable) ||
                HasOptionType(ItemOptionType.Pickable))
            {
                var locked = HasOptionType(ItemOptionType.Locked);
                cb.Add($"var isLocked = {JS.Bool(locked)};");
                cb.Add($"var hasBeenLocked = {JS.Bool(locked)};");
                cb.Add($"var isUnlocked = {JS.Bool(!locked)};");
                cb.Add($"var hasBeenUnlocked = {JS.Bool(!locked)};");
                cb.Add($"var isPicked = false;");
                cb.Add($"var hasBeenPicked = false;");
            }
            cb.Add();
            cb.Add("var additionalVerbs = new collection();");

            cb.Add();
            cb.Add($"var containedItems = new items();");
            foreach (var i in containedItems)
            {
                cb.Add($"containedItems.add({i.Name});");
            }
            cb.Add();

            // The getVerbs() function
            cb.Add("function getVerbs(context) {").In();
            cb.Add("verbs = new collection();");
            cb.Add("verbs.add(\"Examine\", examine);");
            if (!HasOptionType(ItemOptionType.Fixed))
            {
                cb.Add("if (context.inventory().has(this)) {").In();
                cb.Add("verbs.add(\"Drop\", drop);").Out();
                cb.Add("}");
                cb.Add("else {").In();
                cb.Add("verbs.add(\"Take\", take);").Out();
                cb.Add("}");
            }
            if (HasOptionType(ItemOptionType.Openable) || 
                HasOptionType(ItemOptionType.Lockable) ||
                HasOptionType(ItemOptionType.Pickable))
            {
                cb.Add("if (!isClosed) {").In();
                cb.Add("verbs.add(\"Close\", close);").Out();
                cb.Add("}");
                if (HasOptionType(ItemOptionType.Lockable) ||
                    HasOptionType(ItemOptionType.Pickable))
                {
                    cb.Add("if (isClosed && !isLocked) {").In();
                }
                else
                {
                    cb.Add("if (isClosed) {").In();
                }
                cb.Add("verbs.add(\"Open\", open);").Out();
                cb.Add("}");
            }
            if (HasOptionType(ItemOptionType.Lockable) &&
                HasOptionType(ItemOptionType.RequiresKey))
            {
                cb.Add($"if (!isLocked && context.inventory().has({GetRequiredKey()})) {{").In();
                cb.Add("verbs.add(\"Lock\", lock);").Out();
                cb.Add("}");
                cb.Add($"if (isLocked && context.inventory().has({GetRequiredKey()})) {{").In();
                cb.Add("verbs.add(\"Unlock\", unlock);").Out();
                cb.Add("}");
            }
            if (HasOptionType(ItemOptionType.Pickable) &&
                HasOptionType(ItemOptionType.RequiresKey))
            {
                cb.Add($"if (isLocked && context.inventory().has({GetRequiredKey()})) {{").In();
                cb.Add("verbs.add(\"Pick\", pick);").Out();
                cb.Add("}");
            }
            if (HasOptionType(ItemOptionType.Lockable) &&
                HasOptionType(ItemOptionType.RequiresCombination))
            {
                cb.Add("if (isClosed && !isLocked) {").In();
                cb.Add("verbs.add(\"Lock\", lock);").Out();
                cb.Add("}");
                cb.Add("if (isLocked) {").In();
                cb.Add("verbs.add(\"Enter combination\", enterCombination);").Out();
                cb.Add("}");
            }

            foreach (var fn in Functions)
            {
                cb.Add($"if ({fn.Name}.isVisible()) {{ verbs.add(\"{fn.Label}\", {fn.Name}); }}");
            }

            cb.Add("return verbs;").Out();
            cb.Add("}");
            cb.Add();

            // The examine() functions
            cb.Add("function examine(context) {").In();
            cb.Add("_s = \"\";");
            Section.EmitJavaScriptCode(semantics, cb);
            cb.Add("context.setMessage(_s);").Out();
            cb.Add("}");
            cb.Add();

            // The drop(), afterDrop(), take() and afterTake() functions
            if (!HasOptionType(ItemOptionType.Fixed))
            {
                cb.Add("function drop(context) {").In();
                cb.Add($"context.inventory().remove({Name});");
                cb.Add($"context.location().items.add({Name});");
                cb.Add($"context.setMessage(\"You drop the {Label}\");");
                if (HasAction(ItemActionType.AfterDrop))
                {
                    cb.Add("if (afterDrop.isVisible()) {").In();
                    cb.Add("afterDrop(context);").Out();
                    cb.Add("}");
                }
                cb.Out();
                cb.Add("}");
                cb.Add();

                var afterDropAction = GetAction(ItemActionType.AfterDrop);
                if (afterDropAction != null)
                {
                    cb.Add("function afterDrop(context) {").In();
                    cb.Add("var _s = \"\"");
                    afterDropAction.Section.EmitJavaScriptCode(semantics, cb);
                    cb.Add("if (_s != \"\") {").In();
                    cb.Add("context.setMessage(_s);").Out();
                    cb.Add("}").Out();
                    cb.Add("}");
                    cb.Add("afterDrop.visible = true;");
                    cb.Add("afterDrop.isVisible = () => isVisible;");
                    cb.Add("afterDrop.setVisible = value => isVisible = value;");
                }

                var afterTakeAction = GetAction(ItemActionType.AfterTake);
                if (afterTakeAction != null)
                {
                    cb.Add("function afterTake(context) {").In();
                    cb.Add("var _s = \"\"");
                    afterTakeAction.Section.EmitJavaScriptCode(semantics, cb);
                    cb.Add("if (_s != \"\") {").In();
                    cb.Add("context.setMessage(_s);").Out();
                    cb.Add("}").Out();
                    cb.Add("}");
                    cb.Add("afterTake.visible = true;");
                    cb.Add("afterTake.isVisible = () => isVisible;");
                    cb.Add("afterTake.setVisible = value => isVisible = value;");
                }

                cb.Add("function take(context) {").In();
                cb.Add($"context.inventory().remove({Name});");  // If it is contained in a carried object
                cb.Add($"context.inventory().add({Name});");
                cb.Add($"context.historicInventory().add({Name});");
                cb.Add($"context.location().items.remove({Name});");
                cb.Add($"context.setMessage(\"You take the {Label}\");");
                if (HasAction(ItemActionType.AfterTake))
                {
                    cb.Add("if (afterTake.isVisible()) {").In();
                    cb.Add("afterTake(context);").Out();
                    cb.Add("}");
                }
                cb.Out();
                cb.Add("}");
                cb.Add();
            }

            // The open() and close() functions
            if (HasOptionType(ItemOptionType.Openable) || 
                HasOptionType(ItemOptionType.Lockable) ||
                HasOptionType(ItemOptionType.Pickable))
            {
                cb.Add("function open(context) {").In();
                cb.Add($"isOpen = true;");
                cb.Add($"isClosed = false;");
                cb.Add($"hasBeenOpen = true;");
                cb.Add($"context.setMessage(\"You open the {Label}\");").Out();
                cb.Add("}");
                cb.Add();

                cb.Add("function close(context) {").In();
                cb.Add($"isOpen = false;");
                cb.Add($"isClosed = true;");
                cb.Add($"hasBeenClosed = true;");
                cb.Add($"context.setMessage(\"You close the {Label}\");").Out();
                cb.Add("}");
                cb.Add();
            }

            // The pick(), lock(), enterCombination() and unlock() functions
            if (HasOptionType(ItemOptionType.Lockable))
            {
                cb.Add("function lock(context) {").In();
                cb.Add($"isLocked = true;");
                cb.Add($"hasBeenLocked = true;");
                cb.Add($"context.setMessage(\"You lock the {Label}\");").Out();
                cb.Add("}");
                cb.Add();

                if (HasOptionType(ItemOptionType.RequiresCombination))
                {
                    cb.Add("function enterCombination(context, combination) {").In();
                    cb.Add($"keypad.enterCombination(safe, \"{GetRequiredCombination()}\", combination,").In();
                    cb.Add("() => {").In();
                    cb.Add("isLocked = false;");
                    cb.Add("hasBeenUnlocked = true;");
                    cb.Add("context.setMessage(\"You enter the correct combination and unlock the door.\");");
                    cb.Add("view.update(context);").Out();
                    cb.Add("},");
                    cb.Add("() => {").In();
                    cb.Add("context.setMessage(\"For a moment there, you thought you remembered the code. A futile attempt.\");");
                    cb.Add("view.update(context);").Out();
                    cb.Add("});").Out();
                    cb.Add("}").Out();
                    cb.Add();
                }
                else
                {
                    cb.Add("function unlock(context) {").In();
                    cb.Add($"isLocked = false;");
                    cb.Add("hasBeenUnlocked = true;");
                    cb.Add($"context.setMessage(\"You unlock the {Label}\");").Out();
                    cb.Add("}");
                    cb.Add();
                }
            }
            if (HasOptionType(ItemOptionType.Pickable))
            {
                
                cb.Add("function pick(context) {").In();
                cb.Add($"isLocked = false;");
                cb.Add("hasBeenUnlocked = true;");
                cb.Add($"isPicked = false;");
                cb.Add("hasBeenPicked = true;");
                cb.Add($"context.setMessage(\"Using the {semantics.ItemByName(GetRequiredKey()).Label}, you manage to pick the {Label}\");").Out();
                cb.Add("}");
                cb.Add();
            }

            Functions.EmitJavaScript(semantics, cb);

            cb.Add("return {").In();
            cb.Add($"caption: \"{Label}\",");
            cb.Add($"isVisible: () => isVisible,");
            cb.Add($"setVisible: value => isVisible = value,");
            cb.Add($"containedItems: containedItems,");
            cb.Add($"getVerbs: getVerbs,");
            if (HasOptionType(ItemOptionType.Openable) ||
                HasOptionType(ItemOptionType.Lockable) ||
                HasOptionType(ItemOptionType.Pickable))
            {
                cb.Add($"isOpen: () => isOpen,");
                cb.Add($"hasBeenOpen: () => hasBeenOpen,");
                cb.Add($"isClosed: () => isClosed,");
                cb.Add($"hasBeenClosed: () => hasBeenClosed,");
            }
            if (HasOptionType(ItemOptionType.Lockable) ||
                HasOptionType(ItemOptionType.Pickable))
            {
                cb.Add($"isLocked: () => isLocked,");
                cb.Add($"hasBeenLocked: () => hasBeenLocked,");
                cb.Add($"isUnlocked: () => isUnlocked,");
                cb.Add($"hasBeenUnlocked: () => hasBeenUnlocked,");
                cb.Add($"isPicked: () => isPicked,");
                cb.Add($"hasBeenPicked: () => hasBeenPicked,");
            }
            foreach (var fn in Functions)
            {
                cb.Add($"{fn.Name}: {fn.Name},");
            }
            cb.Out();
            cb.Add("}").Out();

            cb.Add("})();");
            cb.Add();
        }

        private List<Item> containedItems;

        private List<string> GetVerbs(bool includeHidden)
        {
            var result = new List<string>();
            result.Add("examine");
            if (Options.Any(i => i.Type == Enumerations.ItemOptionType.Openable))
            {
                result.Add("open");
            }
            foreach (var fun in Functions)
            {
                if (includeHidden || !fun.Hidden)
                    result.Add(fun.Name);
            }
            return result;
        }

        private bool HasOptionType(Enumerations.ItemOptionType optionType)
        {
            return Options.Any(i => i.Type == optionType);
        }

        private ItemAction GetAction(Enumerations.ItemActionType actionType)
        {
            return Actions.FirstOrDefault(i => i.Type == actionType);
        }

        private bool HasAction(Enumerations.ItemActionType actionType)
        {
            return GetAction(actionType) != null;
        }

        private string GetRequiredKey()
        {
            return Options.FirstOrDefault(i => i.Type == Enumerations.ItemOptionType.RequiresKey).Data;
        }

        private string GetRequiredCombination()
        {
            return Options.FirstOrDefault(i => i.Type == Enumerations.ItemOptionType.RequiresCombination).Data;
        }

        private string HasOptionTypeJS(Enumerations.ItemOptionType optionType)
        {
            return HasOptionType(optionType) ? "true" : "false";
        }
    }
}
