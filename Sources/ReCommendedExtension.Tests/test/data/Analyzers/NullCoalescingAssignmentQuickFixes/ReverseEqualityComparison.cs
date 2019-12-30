using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class Sample
    {
        void Method(string x)
        {
            i{caret}f (null == x) x = "four".Remove(0, 1);
        }
    }
}