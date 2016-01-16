using System.Diagnostics.Contracts;

namespace Test
{
    internal class Class
    {
        internal string Method{caret}()
        {
            Contract.Ensures(true);

            return "";
        }
    }
}