using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ConvertToLinqQuery
    {
        public void Method(ICollection<int> source, ICollection<int>? sourceNullable)
        {
            ICollection<int> result11 = sou{on}rce;
            ICollection<int>? result13 = source{off}Nullable;

            List<int> result21 = [..sour{on}ce];
            List<int> result22 = [..source{off}Nullable];

            int[] result31 = sour{on}ce.ToArray();
            int[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<T>(ICollection<T> source, ICollection<T>? sourceNullable)
        {
            ICollection<T> result11 = sou{on}rce;
            ICollection<T>? result13 = source{off}Nullable;

            List<T> result21 = [..sour{on}ce];
            List<T> result22 = [..source{off}Nullable];

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<S, T>(S source, S? sourceNullable) where S : ICollection<T>
        {
            ICollection<T> result11 = sou{on}rce;
            ICollection<T>? result13 = source{off}Nullable;

            List<T> result21 = [..sour{on}ce];
            List<T> result22 = [..source{off}Nullable];

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }
    }
}