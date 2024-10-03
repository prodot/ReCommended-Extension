using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list, int fallback)
        {
            var first1 = list.FirstOrDefault();
            var first2 = list.FirstOrDefault(fallback);

            var last1 = list.LastOrDefault();
            var last2 = list.LastOrDefault(1);

            var single = list.Single();
        }
    }
}