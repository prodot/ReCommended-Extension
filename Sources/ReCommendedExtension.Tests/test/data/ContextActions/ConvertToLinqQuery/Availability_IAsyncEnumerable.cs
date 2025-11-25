using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class ConvertToLinqQuery
    {
        public void Method(IAsyncEnumerable<int> source, IAsyncEnumerable<int>? sourceNullable)
        {
            var result11 = sour{on}ce;
            var result12 = sour{on}ce.Where(item => item > 0);
            var result13 = source{off}Nullable;
            var result14 = from item in sour{off}ce select item;

            int[] result21 = sour{on}ce.ToArrayAsync();
            int[]? result22 = source{off}Nullable?.ToArrayAsync();
        }

        public void Method<T>(IAsyncEnumerable<T> source, IAsyncEnumerable<T>? sourceNullable)
        {
            var result11 = sour{on}ce;
            var result12 = sour{on}ce.Where(item => item.ToString() != "");
            var result13 = source{off}Nullable;
            var result14 = from item in sour{off}ce select item;

            T[] result21 = sour{on}ce.ToArrayAsync();
            T[]? result22 = source{off}Nullable?.ToArrayAsync();
        }

        public void Method<S, T>(S source, S? sourceNullable) where S : IAsyncEnumerable<T>
        {
            var result11 = sour{off}ce;
            var result12 = sour{on}ce.Where(item => item.ToString() != "");
            var result13 = source{off}Nullable;
            var result14 = from item in sour{off}ce select item;

            T[] result21 = sour{on}ce.ToArrayAsync();
            T[]? result22 = source{off}Nullable?.ToArrayAsync();
        }
    }
}