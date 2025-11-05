using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Methods
    {
        public void RangeIndexer(string text, string? textNullable, int[] array, List<int> list, int startIndex, int count)
        {
            var result11 = text.Remove(startIndex);
            var result12 = text.Remove(1);
            var result13 = textNullable?.Remove(startIndex);
            var result14 = textNullable?.Remove(1);
            var result15 = text.Remove(0, count);
            var result16 = textNullable?.Remove(0, count);

            var result21 = list.ElementAt(1);
            var result22 = text.ElementAt(1);
            var result23 = textNullable?.ElementAt(1);
            var result24 = array.ElementAt(1);
        }
    }
}