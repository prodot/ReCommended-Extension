using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class AsyncMethod
    {
        [NotNull]
        async Task Method1() => await Method2();

        [NotNull]
        async Task<int> Method2()
        {
            throw new NotImplementedException();
        }

        [CanBeNull]
        async Task Method3() => await Method4();

        [CanBeNull]
        async Task<int> Method4()
        {
            throw new NotImplementedException();
        }

        async Task Method5() => await Method6();

        async Task<int> Method6()
        {
            throw new NotImplementedException();
        }
    }
}