using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list)
        {
            var first = list.First();
            var last = list.Last();

            var firstByIndex = list.ElementAt(0);
            var lastByIndex = list.ElementAt(^1);
        }
    }
}