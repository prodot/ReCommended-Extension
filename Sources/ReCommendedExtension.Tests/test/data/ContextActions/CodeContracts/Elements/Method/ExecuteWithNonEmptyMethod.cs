using System.Diagnostics.Contracts;

namespace Test
{
    internal class Class
    {
        internal string Method{caret}()
        {
            Contract.Requires(true);

            return "";
        }
    }
}