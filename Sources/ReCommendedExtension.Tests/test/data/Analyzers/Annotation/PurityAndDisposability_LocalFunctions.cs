using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    [method: MustDisposeResource]
    internal ref struct DisposableRefStruct()
    {
        public void Dispose() { }
    }

    internal class Class
    {
        public void NonAnnotated()
        {
            int NonDisposable() => 0;

            IDisposable Disposable() => throw new NotImplementedException();

            IAsyncDisposable AsyncDisposable() => throw new NotImplementedException();

            Task<IDisposable> DisposableTaskLike() => throw new NotImplementedException();

            ValueTask<IAsyncDisposable> AsyncDisposableTaskLike() => throw new NotImplementedException();

            DisposableRefStruct Disposable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithPure()
        {
            [Pure]
            int NonDisposable() => 0;

            [Pure]
            IDisposable Disposable() => throw new NotImplementedException();

            [Pure]
            IAsyncDisposable AsyncDisposable() => throw new NotImplementedException();

            [Pure]
            Task<IDisposable> DisposableTaskLike() => throw new NotImplementedException();

            [Pure]
            ValueTask<IAsyncDisposable> AsyncDisposableTaskLike() => throw new NotImplementedException();

            [Pure]
            DisposableRefStruct Disposable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithMustUseReturnValue()
        {
            [MustUseReturnValue]
            int NonDisposable() => 0;

            [MustUseReturnValue]
            IDisposable Disposable() => throw new NotImplementedException();

            [MustUseReturnValue]
            IAsyncDisposable AsyncDisposable() => throw new NotImplementedException();

            [MustUseReturnValue]
            Task<IDisposable> DisposableTaskLike() => throw new NotImplementedException();

            [MustUseReturnValue]
            ValueTask<IAsyncDisposable> AsyncDisposableTaskLike() => throw new NotImplementedException();

            [MustUseReturnValue]
            DisposableRefStruct Disposable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithMustDisposeResource()
        {
            [MustDisposeResource]
            int NonDisposable() => 0;

            [MustDisposeResource]
            IDisposable Disposable() => throw new NotImplementedException();

            [MustDisposeResource]
            IAsyncDisposable AsyncDisposable() => throw new NotImplementedException();

            [MustDisposeResource]
            Task<IDisposable> DisposableTaskLike() => throw new NotImplementedException();

            [MustDisposeResource]
            ValueTask<IAsyncDisposable> AsyncDisposableTaskLike() => throw new NotImplementedException();

            [MustDisposeResource]
            DisposableRefStruct Disposable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithMustDisposeResourceTrue()
        {
            [MustDisposeResource(true)]
            int NonDisposable() => 0;

            [MustDisposeResource(true)]
            IDisposable Disposable() => throw new NotImplementedException();

            [MustDisposeResource(true)]
            IAsyncDisposable AsyncDisposable() => throw new NotImplementedException();

            [MustDisposeResource(true)]
            Task<IDisposable> DisposableTaskLike() => throw new NotImplementedException();

            [MustDisposeResource(true)]
            ValueTask<IAsyncDisposable> AsyncDisposableTaskLike() => throw new NotImplementedException();

            [MustDisposeResource(true)]
            DisposableRefStruct Disposable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithMustDisposeResourceFalse()
        {
            [MustDisposeResource(false)]
            int NonDisposable() => 0;

            [MustDisposeResource(false)]
            IDisposable Disposable() => throw new NotImplementedException();

            [MustDisposeResource(false)]
            IAsyncDisposable AsyncDisposable() => throw new NotImplementedException();

            [MustDisposeResource(false)]
            Task<IDisposable> DisposableTaskLike() => throw new NotImplementedException();

            [MustDisposeResource(false)]
            ValueTask<IAsyncDisposable> AsyncDisposableTaskLike() => throw new NotImplementedException();

            [MustDisposeResource(false)]
            DisposableRefStruct Disposable2() => throw new NotImplementedException();
        }
    }
}