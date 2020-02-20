using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class Sample
    {
        void Method(string x)
        {
            i{caret}f (x == null) x = new Version(1, 2).ToString();
        }
    }
}