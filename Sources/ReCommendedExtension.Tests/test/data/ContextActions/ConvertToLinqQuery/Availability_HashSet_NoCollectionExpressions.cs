using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ConvertToLinqQuery
    {
        public void Method(HashSet<int> source, HashSet<int>? sourceNullable)
        {
            HashSet<int> result11 = sou{on}rce;
            HashSet<int>? result13 = source{off}Nullable;

            int[] result31 = sour{on}ce.ToArray();
            int[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<T>(HashSet<T> source, HashSet<T>? sourceNullable)
        {
            HashSet<T> result11 = sou{on}rce;
            HashSet<T>? result13 = source{off}Nullable;

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<S, T>(S source, S? sourceNullable) where S : HashSet<T>
        {
            HashSet<T> result11 = sou{off}rce;
            HashSet<T>? result13 = source{off}Nullable;

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }
    }
}