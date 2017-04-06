using System;

namespace Test
{
    internal class UncatchableException
    {
        void Method()
        {
            try { }
            catch (NullReferenceException) { }

            try { }
            catch (NullReferenceException e) when (e.Message != null) { }
        }
    }
}