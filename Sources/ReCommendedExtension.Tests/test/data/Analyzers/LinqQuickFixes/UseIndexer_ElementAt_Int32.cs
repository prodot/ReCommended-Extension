using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list)
        {
            var third = list.Element{caret}At(2);
        }
    }
}