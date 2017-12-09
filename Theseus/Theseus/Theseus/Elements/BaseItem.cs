using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Enumerations;

namespace Theseus.Elements
{
    public class BaseItem
    {
        public string Name { get; }
        public string Label { get; }
        public bool Hidden { get; }
        public IEnumerable<ItemOption> Options { get; }

        public BaseItem(string name, string label, bool hidden, IEnumerable<ItemOption> options)
        {
            Name = name;
            Label = label;
            Hidden = hidden;
            Options = options != null ? options.ToList() : new List<ItemOption>();
        }

        protected bool IsOpenable()
        {
            return HasOptionType(ItemOptionType.Openable) || HasOptionType(ItemOptionType.Lockable);
        }

        protected bool IsClosed()
        {
            return HasOptionType(ItemOptionType.Closed) || HasOptionType(ItemOptionType.Locked);
        }

        protected bool IsFixed()
        {
            return HasOptionType(ItemOptionType.Fixed);
        }

        protected bool IsLocked()
        {
            return HasOptionType(ItemOptionType.Locked);
        }

        protected bool IsLockable()
        {
            return HasOptionType(ItemOptionType.Lockable);
        }

        protected bool IsPickable()
        {
            return HasOptionType(ItemOptionType.Pickable);
        }

        protected bool RequiresKey()
        {
            return HasOptionType(ItemOptionType.RequiresKey);
        }

        protected bool RequiresCombination()
        {
            return HasOptionType(ItemOptionType.RequiresCombination);
        }

        protected string GetRequiredKey()
        {
            var key = Options.FirstOrDefault(i => i.Type == ItemOptionType.RequiresKey);
            return key == null ? "" : key.Data;
        }

        protected string GetRequiredCombination()
        {
            var comb = Options.FirstOrDefault(i => i.Type == ItemOptionType.RequiresCombination);
            return comb == null ? "" : comb.Data;
        }

        private bool HasOptionType(ItemOptionType optionType)
        {
            return Options.Any(i => i.Type == optionType);
        }
    }
}
