using System;
using System.Collections.Generic;

namespace Test
{
    internal class Availability
    {
        void Method()
        {
            var lambda = (IEnumerable<int> o{on}ne, Action tw{on}o, string th{off}ree, ICollection<int> fo{off}ur, IAsyncEnumerable<int> fi{on}fth) => { };

            Action<IEnumerable<int>, Action, string, ICollection<int>, IAsyncEnumerable<int>> lambda2 = (o{on}ne, tw{on}o, th{off}ree, fo{off}ur, fi{on}fth) => { };
        }
    }
}