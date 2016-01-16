using System;
using System.Diagnostics.Contracts;

namespace Test
{
    internal class Class
    {
        internal void Method(string one{caret})
        {
            Contract.Requires(true);

            Console.WriteLine(one);
        }
    }
}