using System;
using System.Threading.Tasks;

namespace Test
{
    internal class Class
    {
        void Method()
        {
            Notify += async delegate { await Task.Yield(); };
            Notify -= async delegate { await Task.Yield(); };

            Notify += async delegate(object sender, EventArgs e) { await Task.Yield(); };
            Notify -= async delegate(object sender, EventArgs e) { await Task.Yield(); };

            EventHandler handler;

            handler = async delegate { await Task.Yield(); };
            handler = async delegate(object sender, EventArgs e) { await Task.Yield(); };

            Notify += handler;
            Notify -= handler;
        }

        event EventHandler Notify;

        void Method2()
        {
            Notify += delegate { };
            Notify -= delegate { };

            Notify += delegate(object sender, EventArgs e) { };
            Notify -= delegate(object sender, EventArgs e) { };

            EventHandler handler;

            handler = delegate { };
            handler = delegate(object sender, EventArgs e) { };

            Notify += handler;
            Notify -= handler;
        }

        event Func<int, Task<int>> EventHandlerWithReturnValue;

        void Method3()
        {
            EventHandlerWithReturnValue += async delegate
            {
                await Task.Yield();
                return 0;
            };
            EventHandlerWithReturnValue -= async delegate
            {
                await Task.Yield();
                return 0;
            };

            EventHandlerWithReturnValue += async delegate(int e)
            {
                await Task.Yield();
                return 0;
            };
            EventHandlerWithReturnValue -= async delegate(int e)
            {
                await Task.Yield();
                return 0;
            };

            Func<int, Task<int>> handler;

            handler = async delegate
            {
                await Task.Yield();
                return 0;
            };
            handler = async delegate(int e)
            {
                await Task.Yield();
                return 0;
            };

            EventHandlerWithReturnValue += handler;
            EventHandlerWithReturnValue -= handler;
        }
    }
}