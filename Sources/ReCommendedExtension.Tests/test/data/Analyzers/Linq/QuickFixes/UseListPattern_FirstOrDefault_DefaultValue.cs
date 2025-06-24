using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list, int? fallback)
        {
            var f = list.Fir{caret}stOrDefault(fallback ?? 3);
        }
    }
}