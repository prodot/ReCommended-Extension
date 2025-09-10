using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ConvertToLinqQuery
    {
        public void Method(List<int> source, List<int>? sourceNullable)
        {
            List<int> result11 = sou{on}rce;
            List<int>? result13 = source{off}Nullable;

            int[] result31 = sour{on}ce.ToArray();
            int[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<T>(List<T> source, List<T>? sourceNullable)
        {
            List<T> result11 = sou{on}rce;
            List<T>? result13 = source{off}Nullable;

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<S, T>(S source, S? sourceNullable) where S : List<T>
        {
            List<T> result11 = sou{on}rce;
            List<T>? result13 = source{off}Nullable;

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }
    }
}