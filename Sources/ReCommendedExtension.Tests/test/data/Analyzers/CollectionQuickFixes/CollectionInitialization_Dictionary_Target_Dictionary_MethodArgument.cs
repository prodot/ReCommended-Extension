using System;
using System.Collections.Generic;

namespace TargetDictionary
{
    public class GenericClass<K, V> where K : struct where V : new()
    {
        void Method(K a, K b, V x, V y, IDictionary<K, V> dict, IEnumerable<KeyValuePair<K, V>> pairs, IEqualityComparer<K> comparer)
        {
            Consumer(ne{caret}w Dictionary<K, V>());
        }

        void Consumer(Dictionary<K, V> items) { }
    }
}