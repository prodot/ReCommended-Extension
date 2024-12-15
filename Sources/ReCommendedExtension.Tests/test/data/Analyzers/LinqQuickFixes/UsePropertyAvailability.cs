using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(ICollection<int> collection, string s, int[] array)
        {
            var count1 = collection.LongCount();
            var count2 = s.LongCount();
            var count3 = array.LongCount();
        }
    }
}