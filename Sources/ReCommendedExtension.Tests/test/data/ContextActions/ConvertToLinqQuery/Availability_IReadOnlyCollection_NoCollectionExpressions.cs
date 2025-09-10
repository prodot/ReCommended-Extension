using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ConvertToLinqQuery
    {
        public void Method(IReadOnlyCollection<int> source, IReadOnlyCollection<int>? sourceNullable)
        {
            IReadOnlyCollection<int> result11 = sou{on}rce;
            IReadOnlyCollection<int>? result13 = source{off}Nullable;

            int[] result31 = sour{on}ce.ToArray();
            int[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<T>(IReadOnlyCollection<T> source, IReadOnlyCollection<T>? sourceNullable)
        {
            IReadOnlyCollection<T> result11 = sou{on}rce;
            IReadOnlyCollection<T>? result13 = source{off}Nullable;

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<S, T>(S source, S? sourceNullable) where S : IReadOnlyCollection<T>
        {
            IReadOnlyCollection<T> result11 = sou{on}rce;
            IReadOnlyCollection<T>? result13 = source{off}Nullable;

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }
    }
}