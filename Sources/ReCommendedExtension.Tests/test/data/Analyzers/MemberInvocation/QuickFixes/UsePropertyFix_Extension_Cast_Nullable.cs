using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Methods
    {
        public void Count(List<int>? list)
        {
            var result = list?.LongCount{caret}();
        }
    }
}