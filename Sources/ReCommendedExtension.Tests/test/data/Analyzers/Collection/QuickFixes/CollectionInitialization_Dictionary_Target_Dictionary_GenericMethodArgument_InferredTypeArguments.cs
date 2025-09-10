using System;
using System.Collections.Generic;

namespace TargetDictionary
{
    public class NonGenericClass
    {
        void Method(int a, int b, string x, string y, IDictionary<int, string> dict, IEnumerable<KeyValuePair<int, string>> pairs, IEqualityComparer<int> comparer)
        {
            ConsumerGeneric(n{caret}ew Dictionary<int, string>());
        }

        void ConsumerGeneric<K, V>(Dictionary<K, V> items) where K : notnull { }
    }
}