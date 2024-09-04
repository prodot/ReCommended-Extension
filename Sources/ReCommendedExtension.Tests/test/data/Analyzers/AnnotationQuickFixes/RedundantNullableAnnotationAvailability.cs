using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class Enumerables
    {
        public IEnumerable<int>? IteratorNullable()
        {
            yield return 1;
        }
    }

    internal class AsyncEnumerables
    {
        public async IAsyncEnumerable<int>? IteratorNullable()
        {
            await Task.Yield();
            yield return 1;
        }
    }
}