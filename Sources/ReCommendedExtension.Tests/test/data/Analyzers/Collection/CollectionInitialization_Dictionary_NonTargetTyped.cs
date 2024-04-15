using System;
using System.Collections.Generic;

namespace NonTargetTyped
{
    public class NonGenericClass
    {
        void Method(int a, int b, string x, string y, IDictionary<int, string> dict, IEnumerable<KeyValuePair<int, string>> pairs, IEqualityComparer<int> comparer)
        {
            var var01 = new Dictionary<int, string>();
            var var02 = new Dictionary<int, string> { { a, x }, { b, y } };
            var var03 = new Dictionary<int, string>(8);
            var var04 = new Dictionary<int, string>(8) { [a] = x, [b] = y };
            var var05 = new Dictionary<int, string>(dict);
            var var06 = new Dictionary<int, string>(dict) { { a, x }, { b, y } };
            var var07 = new Dictionary<int, string>(pairs);
            var var08 = new Dictionary<int, string>(pairs) { [a] = x, [b] = y };
            var var09 = new Dictionary<int, string>(comparer);
            var var10 = new Dictionary<int, string>(comparer) { { a, x }, { b, y } };
            var var11 = new Dictionary<int, string>(8, comparer);
            var var12 = new Dictionary<int, string>(8, comparer) { [a] = x, [b] = y };
            var var13 = new Dictionary<int, string>(dict, comparer);
            var var14 = new Dictionary<int, string>(dict, comparer) { { a, x }, { b, y } };
            var var15 = new Dictionary<int, string>(pairs, comparer);
            var var16 = new Dictionary<int, string>(pairs, comparer) { [a] = x, [b] = y };
        }
    }

    public class GenericClass<K, V> where K : struct where V : new()
    {
        void Method(K a, K b, V x, V y, IDictionary<K, V> dict, IEnumerable<KeyValuePair<K, V>> pairs, IEqualityComparer<K> comparer)
        {
            var var01 = new Dictionary<K, V>();
            var var02 = new Dictionary<K, V> { { a, x }, { b, y } };
            var var03 = new Dictionary<K, V>(8);
            var var04 = new Dictionary<K, V>(8) { [a] = x, [b] = y };
            var var05 = new Dictionary<K, V>(dict);
            var var06 = new Dictionary<K, V>(dict) { { a, x }, { b, y } };
            var var07 = new Dictionary<K, V>(pairs);
            var var08 = new Dictionary<K, V>(pairs) { [a] = x, [b] = y };
            var var09 = new Dictionary<K, V>(comparer);
            var var10 = new Dictionary<K, V>(comparer) { { a, x }, { b, y } };
            var var11 = new Dictionary<K, V>(8, comparer);
            var var12 = new Dictionary<K, V>(8, comparer) { [a] = x, [b] = y };
            var var13 = new Dictionary<K, V>(dict, comparer);
            var var14 = new Dictionary<K, V>(dict, comparer) { { a, x }, { b, y } };
            var var15 = new Dictionary<K, V>(pairs, comparer);
            var var16 = new Dictionary<K, V>(pairs, comparer) { [a] = x, [b] = y };
        }
    }
}