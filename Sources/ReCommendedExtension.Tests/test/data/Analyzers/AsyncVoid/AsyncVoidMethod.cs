using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    public class Class
    {
        public async void UsedPublicMethod()
        {
            await Task.Yield();
        }

        public async void UnusedPublicMethod()
        {
            await Task.Yield();
        }

        [UsedImplicitly]
        public async void UnusedAnnotatedPublicMethod()
        {
            await Task.Yield();
        }

        void Method()
        {
            Notify += AsyncHandler;
            Notify -= AsyncHandler;

            Notify += new EventHandler(AsyncHandler);
            Notify -= new EventHandler(AsyncHandler);

            UsedPublicMethod();

            AsyncHandler2(null, null);
            AsyncHandler3(null, null);
            AsyncHandler3(null, null);
        }

        async void AsyncHandler(object sender, EventArgs e)
        {
            await Task.Yield();
        }

        async void AsyncHandler2(object sender, EventArgs e)
        {
            await Task.Yield();
        }

        async void AsyncHandler3(object sender, EventArgs e)
        {
            await Task.Yield();
        }

        void Method2()
        {
            Notify += SyncHandler;
            Notify -= SyncHandler;

            Notify += new EventHandler(SyncHandler);
            Notify -= new EventHandler(SyncHandler);
        }

        void SyncHandler(object sender, EventArgs e) { }

        async void Handler2(object sender, EventArgs e)
        {
            await Task.Yield();
        }

        event EventHandler Notify;

        void Method3()
        {
            EventHandlerWithReturnValue += AsyncHandler;
            EventHandlerWithReturnValue -= AsyncHandler;

            EventHandlerWithReturnValue += new Func<int, Task<int>>(AsyncHandler);
            EventHandlerWithReturnValue -= new Func<int, Task<int>>(AsyncHandler);
        }

        async Task<int> AsyncHandler(int e)
        {
            await Task.Yield();
            return 0;
        }

        event Func<int, Task<int>> EventHandlerWithReturnValue;
    }

    internal class Base
    {
        internal virtual void Method() { }
    }

    internal interface IBase
    {
        void Method2();
    }

    internal class Derived : Base, IBase
    {
        internal override async void Method() { }

        public async void Method2() { }
    }
}