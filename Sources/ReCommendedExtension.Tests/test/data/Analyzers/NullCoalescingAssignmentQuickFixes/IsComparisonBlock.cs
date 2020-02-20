using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class Sample
    {
        class Data
        {
            string Property { get; set; }
        }

        Data data;

        void Method(string fallbackValue)
        {
            i{caret}f (data.Property is null)
            {
                data.Property = fallbackValue;
            }
        }
    }
}