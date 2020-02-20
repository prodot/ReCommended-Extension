using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class Sample
    {
        void Method(string value)
        {
            i{caret}f (null == value)
            {
                value = string.Join(", ", new[] { "one", "two", "three" });
            }
        }
    }
}