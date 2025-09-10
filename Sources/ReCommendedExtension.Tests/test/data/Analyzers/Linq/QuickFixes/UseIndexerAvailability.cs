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

            var third = list.ElementAt(2);
            var secondFromEnd = list.ElementAt(^2);
        }
    }
}