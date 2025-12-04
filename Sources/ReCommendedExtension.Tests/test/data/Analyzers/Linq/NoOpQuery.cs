using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class LinqQueries
    {
        public void RedundantLinqQuery(IEnumerable<int> items, int[] array, List<int> list)
        {
            var result11 = from item in items select item;
            var result12 = (from item in items select item).ToList();
            var result13 = (from item in array select item).ToList();
            var result14 = (from item in list select item).ToList();

            List<int> result21 = [..from item in items select item];
            List<int> result22 = [..from item in array select item];
            List<int> result23 = [..from item in list select item];

            var result31 = from item in array.AsEnumerable() select item;
            IEnumerable<int> result32 = from item in array select item;
            IEnumerable<int> result33 = from item in list select item;
        }

        public void RedundantLinqQuery<T>(IEnumerable<T> items, T[] array, List<T> list)
        {
            var result11 = from item in items select item;
            var result12 = (from item in items select item).ToList();
            var result13 = (from item in array select item).ToList();
            var result14 = (from item in list select item).ToList();

            List<T> result21 = [..from item in items select item];
            List<T> result22 = [..from item in array select item];
            List<T> result23 = [..from item in list select item];

            var result31 = from item in array.AsEnumerable() select item;
            IEnumerable<T> result32 = from item in array select item;
            IEnumerable<T> result33 = from item in list select item;
        }

        public void NoDetection(IEnumerable<int> items, int[] array, List<int> list)
        {
            var result11 = from int item in items select item;
            var result12 = from item in items where item > 0 select item;
            var result13 = from item in items select item.ToString();

            var result21 = from item in array select item;
            var result22 = from item in list select item;
        }

        public void NoDetection<T>(IEnumerable<T> items, T[] array, List<T> list)
        {
            var result11 = from T item in items select item;
            var result12 = from item in items where item.ToString()?.Length > 0 select item;
            var result13 = from item in items select item.ToString();

            var result21 = from item in array select item;
            var result22 = from item in list select item;
        }
    }
}