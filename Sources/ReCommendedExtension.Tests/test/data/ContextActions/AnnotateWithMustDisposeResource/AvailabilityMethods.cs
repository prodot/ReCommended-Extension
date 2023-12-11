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
        public void Void{off}Method() { }

        public IDisposable IDisposable{on}Method() => throw new NotImplementedException();

        public Task<IDisposable> IDisposable{on}MethodAsync() => throw new NotImplementedException();

        public ValueTask<IDisposable> IDisposable{on}MethodAsync2() => throw new NotImplementedException();

        public Disposable Disposable{on}Method() => throw new NotImplementedException();

        public Task<Disposable> Disposable{on}MethodAsync() => throw new NotImplementedException();

        public ValueTask<Disposable> Disposable{on}MethodAsync2() => throw new NotImplementedException();

        public IAsyncDisposable IAsyncDisposable{on}Method() => throw new NotImplementedException();

        public Task<IAsyncDisposable> IAsyncDisposable{on}MethodAsync() => throw new NotImplementedException();

        public ValueTask<IAsyncDisposable> IAsyncDisposable{on}MethodAsync2() => throw new NotImplementedException();

        public AsyncDisposable AsyncDisposable{on}Method() => throw new NotImplementedException();

        public Task<AsyncDisposable> AsyncDisposable{on}MethodAsync() => throw new NotImplementedException();

        public ValueTask<AsyncDisposable> AsyncDisposable{on}MethodAsync2() => throw new NotImplementedException();

        public Task Task{off}Method() => throw new NotImplementedException();
        public Task<int> TaskMethod{off}2() => throw new NotImplementedException();
    }
}