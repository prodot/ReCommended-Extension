using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Methods
{
    [MustDisposeResource]
    class Disposable: IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    class AsyncDisposable : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    class Test
    {
        public void MethodInput(
            int p{off}0, 
            IDisposable p{off}1,
            Task<IDisposable> p{off}2,
            ValueTask<IDisposable> p{off}3,
            Disposable p{off}4,
            Task<Disposable> p{off}5,
            ValueTask<Disposable> p{off}6,
            IAsyncDisposable p{off}7,
            Task<IAsyncDisposable> p{off}8,
            ValueTask<IAsyncDisposable> p{off}9,
            AsyncDisposable p{off}10,
            Task<AsyncDisposable> p{off}11,
            ValueTask<AsyncDisposable> p{off}12,
            Task p{off}13, 
            Task<int> p{off}14) { }

        public void MethodInput2(
            in int p{off}0,
            in IDisposable p{off}1,
            in Task<IDisposable> p{off}2,
            in ValueTask<IDisposable> p{off}3,
            in Disposable p{off}4,
            in Task<Disposable> p{off}5,
            in ValueTask<Disposable> p{off}6,
            in IAsyncDisposable p{off}7,
            in Task<IAsyncDisposable> p{off}8,
            in ValueTask<IAsyncDisposable> p{off}9,
            in AsyncDisposable p{off}10,
            in Task<AsyncDisposable> p{off}11,
            in ValueTask<AsyncDisposable> p{off}12,
            in Task p{off}13, 
            in Task<int> p{off}14) { }

        public void MethodInput3(
            ref readonly int p{off}0,
            ref readonly IDisposable p{off}1,
            ref readonly Task<IDisposable> p{off}2,
            ref readonly ValueTask<IDisposable> p{off}3,
            ref readonly Disposable p{off}4,
            ref readonly Task<Disposable> p{off}5,
            ref readonly ValueTask<Disposable> p{off}6,
            ref readonly IAsyncDisposable p{off}7,
            ref readonly Task<IAsyncDisposable> p{off}8,
            ref readonly ValueTask<IAsyncDisposable> p{off}9,
            ref readonly AsyncDisposable p{off}10,
            ref readonly Task<AsyncDisposable> p{off}11,
            ref readonly ValueTask<AsyncDisposable> p{off}12,
            ref readonly Task p {off}13, 
            ref readonly Task<int> p{off}14) { }

        public void MethodInputOutput(
            ref int p{off}0,
            ref IDisposable p{on}1,
            ref Task<IDisposable> p{on}2,
            ref ValueTask<IDisposable> p{on}3,
            ref Disposable p{on}4,
            ref Task<Disposable> p{on}5,
            ref ValueTask<Disposable> p{on}6,
            ref IAsyncDisposable p{on}7,
            ref Task<IAsyncDisposable> p{on}8,
            ref ValueTask<IAsyncDisposable> p{on}9,
            ref AsyncDisposable p{on}10,
            ref Task<AsyncDisposable> p{on}11,
            ref ValueTask<AsyncDisposable> p{on}12,
            ref Task p{off}13, 
            ref Task<int> p{off}14) { }

        public void MethodOutput(
            out int p{off}0,
            out IDisposable p{on}1,
            out Task<IDisposable> p{on}2,
            out ValueTask<IDisposable> p{on}3,
            out Disposable p{on}4,
            out Task<Disposable> p{on}5,
            out ValueTask<Disposable> p{on}6,
            out IAsyncDisposable p{on}7,
            out Task<IAsyncDisposable> p{on}8,
            out ValueTask<IAsyncDisposable> p{on}9,
            out AsyncDisposable p{on}10,
            out Task<AsyncDisposable> p{on}11,
            out ValueTask<AsyncDisposable> p{on}12,
            out Task p{off}13, 
            out Task<int> p{off}14) => throw new NotImplementedException();
    }
}