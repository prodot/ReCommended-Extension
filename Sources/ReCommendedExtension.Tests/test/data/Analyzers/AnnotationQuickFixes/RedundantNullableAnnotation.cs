﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class Enumerables
    {
        public IEnumerable<int>?{caret} IteratorNullable()
        {
            yield return 1;
        }
    }
}