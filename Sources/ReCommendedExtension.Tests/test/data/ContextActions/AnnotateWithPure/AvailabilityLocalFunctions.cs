using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal ref struct DisposableRefStruct
    {
        public void Dispose() { }
    }

    internal class Class
    {
        public void NonAnnotated()
        {
            int Non{on}Disposable() => 0;

            IDisposable Dispo{off}sable() => throw new NotImplementedException();

            IAsyncDisposable Async{off}Disposable() => throw new NotImplementedException();

            Task<IDisposable> Disposable{off}TaskLike() => throw new NotImplementedException();

            ValueTask<IAsyncDisposable> AsyncDisposable{off}TaskLike() => throw new NotImplementedException();

            DisposableRefStruct Dispo{off}sable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithPure()
        {
            [Pure]
            int Non{off}Disposable() => 0;

            [Pure]
            IDisposable Dispo{off}sable() => throw new NotImplementedException();

            [Pure]
            IAsyncDisposable Async{off}Disposable() => throw new NotImplementedException();

            [Pure]
            Task<IDisposable> Disposable{off}TaskLike() => throw new NotImplementedException();

            [Pure]
            ValueTask<IAsyncDisposable> AsyncDisposable{off}TaskLike() => throw new NotImplementedException();

            [Pure]
            DisposableRefStruct Dispo{off}sable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithMustUseReturnValue()
        {
            [MustUseReturnValue]
            int Non{on}Disposable() => 0;

            [MustUseReturnValue]
            IDisposable Dispo{off}sable() => throw new NotImplementedException();

            [MustUseReturnValue]
            IAsyncDisposable Async{off}Disposable() => throw new NotImplementedException();

            [MustUseReturnValue]
            Task<IDisposable> Disposable{off}TaskLike() => throw new NotImplementedException();

            [MustUseReturnValue]
            ValueTask<IAsyncDisposable> AsyncDisposable{off}TaskLike() => throw new NotImplementedException();

            [MustUseReturnValue]
            DisposableRefStruct Dispo{off}sable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithMustDisposeResource()
        {
            [MustDisposeResource]
            int Non{on}Disposable() => 0;

            [MustDisposeResource]
            IDisposable Dispo{off}sable() => throw new NotImplementedException();

            [MustDisposeResource]
            IAsyncDisposable Async{off}Disposable() => throw new NotImplementedException();

            [MustDisposeResource]
            Task<IDisposable> Disposable{off}TaskLike() => throw new NotImplementedException();

            [MustDisposeResource]
            ValueTask<IAsyncDisposable> AsyncDisposable{off}TaskLike() => throw new NotImplementedException();

            [MustDisposeResource]
            DisposableRefStruct Dispo{off}sable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithMustDisposeResourceTrue()
        {
            [MustDisposeResource(true)]
            int Non{on}Disposable() => 0;

            [MustDisposeResource(true)]
            IDisposable Dispo{off}sable() => throw new NotImplementedException();

            [MustDisposeResource(true)]
            IAsyncDisposable Async{off}Disposable() => throw new NotImplementedException();

            [MustDisposeResource(true)]
            Task<IDisposable> Disposable{off}TaskLike() => throw new NotImplementedException();

            [MustDisposeResource(true)]
            ValueTask<IAsyncDisposable> AsyncDisposable{off}TaskLike() => throw new NotImplementedException();

            [MustDisposeResource(true)]
            DisposableRefStruct Dispo{off}sable2() => throw new NotImplementedException();
        }

        public void AnnotatedWithMustDisposeResourceFalse()
        {
            [MustDisposeResource(false)]
            int Non{on}Disposable() => 0;

            [MustDisposeResource(false)]
            IDisposable Dispo{off}sable() => throw new NotImplementedException();

            [MustDisposeResource(false)]
            IAsyncDisposable Async{off}Disposable() => throw new NotImplementedException();

            [MustDisposeResource(false)]
            Task<IDisposable> Disposable{off}TaskLike() => throw new NotImplementedException();

            [MustDisposeResource(false)]
            ValueTask<IAsyncDisposable> AsyncDisposable{off}TaskLike() => throw new NotImplementedException();

            [MustDisposeResource(false)]
            DisposableRefStruct Dispo{off}sable2() => throw new NotImplementedException();
        }
    }
}