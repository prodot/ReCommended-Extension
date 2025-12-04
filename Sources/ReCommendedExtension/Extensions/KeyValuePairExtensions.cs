using System.ComponentModel;

namespace ReCommendedExtension.Extensions;

internal static class KeyValuePairExtensions
{
    extension<K, V>(KeyValuePair<K, V> pair)
    {
        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out K key, out V value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}