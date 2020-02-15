using System.Collections.Generic;

namespace EasyMessage.Core.Extension
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.TryGetValue(key, out var value) ? value : default(TValue);
        }

    }
}