using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal record NonDisposable
    {
        public void Clo{off}se() { }
    }

    internal class DisposableClass : IDisposable
    {
        public void Dispo{off}se() { }

        public void Clo{on}se() { }

        [HandlesResourceDisposal]
        public virtual void Close{off}Annotated() { }

        public static void Static{off}Method() { }

        void Private{off}Method() { }
    }

    internal class DerivedClass : DisposableClass
    {
        public override void Close{off}Annotated() { }
    }

    internal class AsyncDisposableClass : IAsyncDisposable
    {
        public ValueTask Dispose{off}Async() => ValueTask.CompletedTask;

        public ValueTask Clo{on}se() => ValueTask.CompletedTask;
    }

    internal ref struct DisposableRefStruct
    {
        public void Dispo{off}se() { }

        public ValueTask Dispose{off}Async() => ValueTask.CompletedTask;

        public ValueTask Clo{on}se() => ValueTask.CompletedTask;
    }
}