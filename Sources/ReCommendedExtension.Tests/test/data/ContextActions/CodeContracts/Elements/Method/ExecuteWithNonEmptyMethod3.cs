using System;
using System.Diagnostics.Contracts;

namespace Test
{
    internal class Class
    {
        internal string Method{caret}()
        {
            Contract.EnsuresOnThrow<InvalidOperationException>(true);

            return "";
        }
    }
}