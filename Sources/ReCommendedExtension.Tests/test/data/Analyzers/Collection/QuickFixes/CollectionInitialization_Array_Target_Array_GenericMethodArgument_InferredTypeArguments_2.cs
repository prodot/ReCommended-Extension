using System;

namespace TargetArray
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c)
        {
            ConsumerGeneric(new int{caret}[0]);
        }

        void ConsumerGeneric<T>(T[] items) { }
    }
}