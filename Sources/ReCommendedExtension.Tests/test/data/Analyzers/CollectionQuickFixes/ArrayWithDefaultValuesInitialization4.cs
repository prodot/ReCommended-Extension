using System.Collections.Generic;

namespace Test
{
    public class GenericClass<T>
    {
        void Method()
        {
            var variable = new[] { null, default, default(List<T>) {caret}};
        }
    }
}