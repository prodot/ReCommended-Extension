using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<bool> list)
        {
            var f = !list.Sing{caret}leOrDefault();
        }
    }
}