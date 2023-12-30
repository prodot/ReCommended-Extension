using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal record NonDisposable
    {
        [HandlesResourceDisposal]
        public void Close() { }
    }

    [MustDisposeResource]
    internal class DisposableClass : IDisposable
    {
        [HandlesResourceDisposal]
        public void Dispose() { }

        [HandlesResourceDisposal]
        public void Close() { }

        [HandlesResourceDisposal]
        public virtual void CloseAnnotated() { }

        [HandlesResourceDisposal]
        public static void StaticMethod() { }

        [HandlesResourceDisposal]
        void PrivateMethod() { }
    }

    internal class DerivedClass : DisposableClass
    {
        [HandlesResourceDisposal]
        public override void CloseAnnotated() { }
    }

    [MustDisposeResource]
    internal class AsyncDisposableClass : IAsyncDisposable
    {
        [HandlesResourceDisposal]
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [HandlesResourceDisposal]
        public ValueTask Close() => ValueTask.CompletedTask;
    }

    [method: MustDisposeResource]
    internal ref struct DisposableRefStruct()
    {
        [HandlesResourceDisposal]
        public void Dispose() { }

        [HandlesResourceDisposal]
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [HandlesResourceDisposal]
        public ValueTask Close() => ValueTask.CompletedTask;
    }
}