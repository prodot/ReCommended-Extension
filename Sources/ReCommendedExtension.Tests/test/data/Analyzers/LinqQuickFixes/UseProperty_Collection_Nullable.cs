using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(ICollection<int>? collection)
        {
            var count = collection?.Long{caret}Count();
        }
    }
}