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

    internal class AsyncMethods
    {
        public async Task? AsyncTask1() => await Task.Yield();

        public async Task? AsyncTask2()
        {
            await Task.Yield();
            Console.WriteLine();
        } 

        public async Task<int>? AsyncTaskWithResult()
        {
            await Task.Yield();
            return 1;
        }
    }

    internal static class LocalFunctions
    {
        public static void Enumerables()
        {
            IEnumerable<int>? NonIterator() => null;

            IEnumerable<int>? IteratorNullable()
            {
                yield return 1;
            }
        }

        public static void Iterators()
        {
            [MustDisposeResource(false)]
            IEnumerator<int>? NonIterator() => null;

            [MustDisposeResource(false)]
            IEnumerator<int>? IteratorNullable()
            {
                yield return 1;
            }
        }

        public static void AsyncEnumerables()
        {
            IAsyncEnumerable<int>? NonIterator() => null;

            async IAsyncEnumerable<int>? IteratorNullable()
            {
                await Task.Yield();
                yield return 1;
            }
        }

        internal static void AsyncIterators()
        {
            [MustDisposeResource(false)]
            IAsyncEnumerator<int>? NonIterator() => null;

            [MustDisposeResource(false)]
            async IAsyncEnumerator<int>? IteratorNullable()
            {
                await Task.Yield();
                yield return 1;
            }
        }

        internal static void AsyncMethods()
        {
            async Task? AsyncTask1() => await Task.Yield();

            async Task? AsyncTask2()
            {
                await Task.Yield();
                Console.WriteLine();
            }

            async Task<int>? AsyncTaskWithResult()
            {
                await Task.Yield();
                return 1;
            }
        }
    }
}