using System.Collections.Generic;

namespace Test
{
    internal class Availability
    {
        IEnumerable<string> Meth{on}od(IEnumerable<int> arg) => null;

        void MethodWithLocalFunction()
        {
            IEnumerable<string> Local{on}Function(IEnumerable<int> arg) => null;
        }

        IEnumerable<string> Method{off}2() => null;

        void Method{off}3() => null;

        IEnumerable<string> Proper{off}ty => null;
    }
}