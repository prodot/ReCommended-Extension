using System;

namespace Test
{
    internal class Foo
    {
        void Method()
        { 
            var lambda = (int fi{on}rst, uint sec{off}ond) => { };
            Action<int> lambda2 = fir{on}st => { };
            Action<int, uint> lambda3 = (fir{on}st, sec{off}ond) => { };

            var ad = delegate (int fi{off}rst) { }
        }
    }
}