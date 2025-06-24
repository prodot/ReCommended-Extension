using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(string? collection)
        {
            var count = collection?.Long{caret}Count();
        }
    }
}