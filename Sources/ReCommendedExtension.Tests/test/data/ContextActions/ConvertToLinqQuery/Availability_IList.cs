using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ConvertToLinqQuery
    {
        public void Method(IList<int> source, IList<int>? sourceNullable)
        {
            IList<int> result11 = sou{on}rce;
            IList<int>? result13 = source{off}Nullable;

            List<int> result21 = [..sour{on}ce];
            List<int> result22 = [..source{off}Nullable];

            int[] result31 = sour{on}ce.ToArray();
            int[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<T>(IList<T> source, IList<T>? sourceNullable)
        {
            IList<T> result11 = sou{on}rce;
            IList<T>? result13 = source{off}Nullable;

            List<T> result21 = [..sour{on}ce];
            List<T> result22 = [..source{off}Nullable];

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<S, T>(S source, S? sourceNullable) where S : IList<T>
        {
            IList<T> result11 = sou{on}rce;
            IList<T>? result13 = source{off}Nullable;

            List<T> result21 = [..sour{on}ce];
            List<T> result22 = [..source{off}Nullable];

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }
    }
}