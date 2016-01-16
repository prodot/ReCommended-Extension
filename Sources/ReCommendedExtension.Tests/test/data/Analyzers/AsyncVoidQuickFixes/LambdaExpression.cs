using System;
using System.Threading.Tasks;

namespace Test
{
    internal class Class
    {
        void Method()
        {
            EventHandler handler = as{caret}ync (sender, args) => await Task.Yield();
        }

        event EventHandler Notify;
    }
}