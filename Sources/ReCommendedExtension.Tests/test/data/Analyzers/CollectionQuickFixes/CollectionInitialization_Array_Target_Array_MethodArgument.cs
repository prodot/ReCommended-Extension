using System;

namespace TargetArray
{
    public class GenericClass<T> where T : new()
    {
        void Method(T a, T b, T c)
        {
            Consumer(new T{caret}[0]);
        }

        void Consumer(T[] items) { }
    }
}