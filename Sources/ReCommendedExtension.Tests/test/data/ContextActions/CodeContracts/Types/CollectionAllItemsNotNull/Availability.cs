using System;
using System.Collections;
using System.Collections.Generic;

namespace Test
{
    internal class Availability
    {
        void Available(ICollection<string> one{on}, string[] two{on}, IDictionary<int, string> three{on}, ICollection four{on}) { }

        void NotAvailable(IEnumerable<string> one{off}, Array two{off}, ICollection<int> three{off}, int[] four{off}, IDictionary<int, int> five{off}) { }
    }
}