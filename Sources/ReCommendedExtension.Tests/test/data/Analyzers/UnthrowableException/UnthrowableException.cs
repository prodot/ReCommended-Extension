using System;

namespace Test
{
    internal class UnthrowableException
    {
        void Method()
        {
            throw new Exception();
        }
    }
}