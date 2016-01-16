using System;

namespace Test
{
    internal class Availability
    {
        void Available(IntPtr one{on}, UIntPtr two{on}) { }

        void NotAvailable(int one{off}) { }
    }
}