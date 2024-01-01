using System.Collections.Generic;

namespace Test
{
    public class GenericClass<T>
    {
        void Method()
        {
            var array = new List<T>[] {caret}{ };
        }
    }
}