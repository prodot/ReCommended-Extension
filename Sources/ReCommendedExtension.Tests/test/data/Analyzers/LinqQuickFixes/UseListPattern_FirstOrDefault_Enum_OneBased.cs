﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    enum CustomEnum
    {
        One = 1,
        Two = 2,
        Three = 3
    }

    public class Class
    {
        public void Method(IList<CustomEnum> list)
        {
            var f = list.Fir{caret}stOrDefault();
        }
    }
}