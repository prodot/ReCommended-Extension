using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list, int fallback)
        {
            var single1 = list.SingleOrDefault();
            var single2 = list.SingleOrDefault(fallback);
        }
    }
}