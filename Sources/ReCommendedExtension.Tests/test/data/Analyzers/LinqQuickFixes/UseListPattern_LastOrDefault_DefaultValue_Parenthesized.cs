﻿using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Class
    {
        public void Method(IList<int> list)
        {
            var f = list.La{caret}stOrDefault(-1) is >= 0 and < 3;
        }
    }
}