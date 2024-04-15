using System.Collections.Generic;

namespace Test
{
    internal class Availability
    {
        IAsyncEnumerable<string> Meth{on}od(IAsyncEnumerable<int> arg) => null;

        void MethodWithLocalFunction()
        {
            IAsyncEnumerable<string> Local{on}Function(IAsyncEnumerable<int> arg) => null;
        }

        IAsyncEnumerable<string> Method{off}2() => null;

        void Method{off}3() => null;

        IAsyncEnumerable<string> Proper{off}ty => null;
    }
}