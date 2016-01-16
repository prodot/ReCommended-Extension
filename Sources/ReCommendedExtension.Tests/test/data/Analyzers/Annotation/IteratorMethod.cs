using System.Collections.Generic;
using JetBrains.Annotations;

namespace Test
{
    internal class IteratorMethod
    {
        [NotNull]
        IEnumerable<int> Method1()
        {
            yield break;
        }

        [CanBeNull]
        IEnumerable<int> Method2()
        {
            yield break;
        }

        IEnumerable<int> Method3()
        {
            yield break;
        }
    }
}