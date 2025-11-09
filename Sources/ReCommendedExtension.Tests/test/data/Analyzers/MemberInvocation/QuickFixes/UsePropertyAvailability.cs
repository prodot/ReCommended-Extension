using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Methods
    {
        public void LastIndexOf(string text, StringComparison comparisonType, object someObject, int[] array, List<int> list)
        {
            var result11 = text.LastIndexOf("");
            var result12 = text.LastIndexOf("", comparisonType);
            var result13 = someObject.ToString().LastIndexOf("", comparisonType);

            var result21 = text.LongCount();
            var result22 = array.LongCount();
            var result23 = list.LongCount();
        }
    }
}