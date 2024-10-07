using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list, int[] array)
        {
            var f = array[^list.Element{caret}At(0)];
        }
    }
}