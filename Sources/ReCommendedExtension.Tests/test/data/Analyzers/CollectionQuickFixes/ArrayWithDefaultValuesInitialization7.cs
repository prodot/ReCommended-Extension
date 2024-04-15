using System.Collections.Generic;

namespace Test
{
    public class GenericClass<T> where T : class
    {
        void Method()
        {
            T?[] variable = [null, default, default(T){caret}];
        }
    }
}