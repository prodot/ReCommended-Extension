using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list)
        {
            var last = list.La{caret}st();
        }
    }
}