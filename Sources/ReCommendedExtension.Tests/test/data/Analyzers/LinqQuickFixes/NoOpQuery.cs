using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void RedundantLinqQuery(IEnumerable<int> items)
        {
            var result = from item in items select{caret} item;
        }
    }
}