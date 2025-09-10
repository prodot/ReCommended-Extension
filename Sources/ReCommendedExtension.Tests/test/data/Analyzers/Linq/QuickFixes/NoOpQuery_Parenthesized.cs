using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void RedundantLinqQuery(IEnumerable<int> items)
        {
            var result = (from {caret}item in items select item).ToList();
        }
    }
}