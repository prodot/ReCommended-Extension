using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Methods
    {
        public void Count(List<int>? list)
        {
            long? result = list?.LongCount{caret}();
        }
    }
}