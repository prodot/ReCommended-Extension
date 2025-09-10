using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<string> list)
        {
            var f = list.La{caret}stOrDefault() ?? "one";
        }
    }
}