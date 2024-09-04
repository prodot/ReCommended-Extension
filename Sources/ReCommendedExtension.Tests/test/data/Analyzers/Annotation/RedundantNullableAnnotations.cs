using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class Enumerables
    {
        public IEnumerable<int>? NonIterator() => null;

        public IEnumerable<int>? IteratorNullable()
        {
            yield return 1;
        }
    }

    internal class Iterators
    {
        [MustDisposeResource(false)]
        public IEnumerator<int>? NonIterator() => null;

        [MustDisposeResource(false)]
        public IEnumerator<int>? IteratorNullable()
        {
            yield return 1;
        }
    }

    internal class AsyncEnumerables
    {
        public IAsyncEnumerable<int>? NonIterator() => null;

        public async IAsyncEnumerable<int>? IteratorNullable()
        {
            await Task.Yield();
            yield return 1;
        }
    }

    internal class AsyncIterators
    {
        [MustDisposeResource(false)]
        public IAsyncEnumerator<int>? NonIterator() => null;

        [MustDisposeResource(false)]
        public async IAsyncEnumerator<int>? IteratorNullable()
        {
            await Task.Yield();
            yield return 1;
        }
    }

}