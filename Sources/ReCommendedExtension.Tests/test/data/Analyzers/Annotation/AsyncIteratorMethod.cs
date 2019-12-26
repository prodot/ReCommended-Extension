using System.Collections.Generic;
using JetBrains.Annotations;

namespace Test
{
    internal class AsyncIteratorMethod
    {
        [NotNull]
        async IAsyncEnumerable<int> Method1()
        {
            yield break;
        }

        [CanBeNull]
        async IAsyncEnumerable<int> Method2()
        {
            yield break;
        }

        async IAsyncEnumerable<int> Method3()
        {
            yield break;
        }
    }
}