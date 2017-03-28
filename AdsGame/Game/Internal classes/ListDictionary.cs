namespace Ads.Game.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    class ListDictionary<TKey,TValue>
    {
        Dictionary<TKey, List<TValue>> Dict;

        public ListDictionary()
        {
            Dict = new Dictionary<TKey, List<TValue>>();
        }

        public List<TValue> GetList(TKey key)
        {
            List<TValue> values;
            if (!Dict.TryGetValue(key, out values))
            {
                values = new List<TValue>();
                Dict.Add(key, values);
            }
            return values;
        }

        public void Add(TKey key, TValue value)
        {
            GetList(key).Add(value);
        }

        public void Remove(TKey key, TValue value)
        {
            List<TValue> values;
            if (Dict.TryGetValue(key, out values))
            {
                values.Remove(value);
                if (values.Count == 0)
                {
                    Dict.Remove(key);
                }
            }
        }

        public bool TryGetValues(TKey key, out List<TValue> values)
        {
            return Dict.TryGetValue(key, out values);
        }

        public IEnumerable<TValue> GetAllValues()
        {
            IEnumerable<TValue> result = new List<TValue>();
            foreach (var list in Dict.Values)
            {
                result = result.Concat(list);
            }
            return result;
        }
    }
}
