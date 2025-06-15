using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ConvertToLinqQuery
    {
        public void Method(IEnumerable<int> source, IEnumerable<int>? sourceNullable)
        {
            var result11 = sour{on}ce;
            var result12 = sour{on}ce.Where(item => item > 0);
            var result13 = source{off}Nullable;
            var result14 = from item in sour{off}ce select item;

            List<int> result21 = [..sour{on}ce];
            List<int> result22 = [..source{off}Nullable];

            int[] result31 = sour{on}ce.ToArray();
            int[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<T>(IEnumerable<T> source, IEnumerable<T>? sourceNullable)
        {
            var result11 = sour{on}ce;
            var result12 = sour{on}ce.Where(item => item.ToString() != "");
            var result13 = source{off}Nullable;
            var result14 = from item in sour{off}ce select item;

            List<T> result21 = [..sour{on}ce];
            List<T> result22 = [..source{off}Nullable];

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }

        public void Method<S, T>(S source, S? sourceNullable) where S : IEnumerable<T>
        {
            var result11 = sour{off}ce;
            var result12 = sour{on}ce.Where(item => item.ToString() != "");
            var result13 = source{off}Nullable;
            var result14 = from item in sour{off}ce select item;

            List<T> result21 = [..sour{on}ce];
            List<T> result22 = [..source{off}Nullable];

            T[] result31 = sour{on}ce.ToArray();
            T[]? result32 = source{off}Nullable?.ToArray();
        }
    }
}