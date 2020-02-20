using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class Sample
    {
        string Property { get; set; }

        void Method(string fallbackValue)
        {
            i{caret}f (Property is null) Property = fallbackValue.Trim();
        }
    }
}