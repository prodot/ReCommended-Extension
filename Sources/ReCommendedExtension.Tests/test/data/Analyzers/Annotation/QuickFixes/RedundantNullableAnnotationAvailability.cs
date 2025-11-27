using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    internal class AsyncMethods
    {
        public async Task? AsyncTask() => await Task.Yield();

        public async Task<int>? AsyncTaskWithResult()
        {
            await Task.Yield();
            return 1;
        }
    }
}