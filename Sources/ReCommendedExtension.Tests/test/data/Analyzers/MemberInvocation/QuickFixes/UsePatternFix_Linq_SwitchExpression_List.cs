using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Methods
    {
        public void SingleOrDefault(List<int> list)
        {
            var result = list.SingleOrDefault({caret});
        }
    }
}