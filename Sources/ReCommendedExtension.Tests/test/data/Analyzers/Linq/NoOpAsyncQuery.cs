using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class LinqQueries
    {
        public void RedundantLinqQuery(IAsyncEnumerable<int> items)
        {
            var result11 = from item in items select item;
            var result12 = (from item in items select item).ToListAsync();

            var result31 = from item in items.Where(i => i > 0) select item;
            IAsyncEnumerable<int> result32 = from item in items.Order() select item;
        }

        public void RedundantLinqQuery<T>(IAsyncEnumerable<T> items)
        {
            var result11 = from item in items select item;
            var result12 = (from item in items select item).ToListAsync();

            var result31 = from item in items.Where(i => i.ToString() != "") select item;
            IAsyncEnumerable<T> result32 = from item in items.Order() select item;
        }

        public void NoDetection(IAsyncEnumerable<int> items)
        {
            var result11 = from int item in items select item;
            var result12 = from item in items where item > 0 select item;
            var result13 = from item in items select item.ToString();

            var result21 = from item in items.Order() select item;
        }

        public void NoDetection<T>(IAsyncEnumerable<T> items)
        {
            var result11 = from T item in items select item;
            var result12 = from item in items where item.ToString()?.Length > 0 select item;
            var result13 = from item in items select item.ToString();

            var result21 = from item in items.Order() select item;
        }
    }
}