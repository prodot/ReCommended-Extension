using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<DayOfWeek> list)
        {
            var f = list.Fir{caret}stOrDefault();
        }
    }
}