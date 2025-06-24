using System;

namespace TargetArray
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c)
        {
            ConsumerGeneric(new {caret}int[] { });
        }

        void ConsumerGeneric<T>(T[] items) { }
    }
}