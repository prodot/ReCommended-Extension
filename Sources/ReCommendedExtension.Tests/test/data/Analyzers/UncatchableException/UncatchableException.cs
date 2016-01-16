using System;

namespace Test
{
    internal class UncatchableException
    {
        void Method()
        {
            try { }
            catch (NullReferenceException) { }
        }
    }
}