using System;
using System.Collections;
using System.Collections.Generic;

namespace Test
{
    internal class Availability
    {
        void Available(ICollection<string> one{on}, string[] two{on}, IDictionary<int, string> three{on}, ICollection four{on}, Array five{on}) { }

        void NotAvailable(IEnumerable<string> one{off}, string two{off}) { }
    }
}