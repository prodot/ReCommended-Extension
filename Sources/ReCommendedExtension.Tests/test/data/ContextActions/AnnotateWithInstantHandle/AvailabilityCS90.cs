using System;
using System.Collections.Generic;

namespace Test
{
    internal class Availability
    {
        void Method(IEnumerable<int> o{on}ne, Action tw{on}o, string th{off}ree, ICollection<int> fo{off}ur)
        {
            void LocalFunction(IEnumerable<int> fir{on}st, Action se{on}cond, string th{off}ird, ICollection<int> fou{off}rth) { }

            static void LocalFunction2(IEnumerable<int> fir{on}st, Action se{on}cond, string th{off}ird, ICollection<int> fou{off}rth) { }
        }
    }
}