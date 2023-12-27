using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    [method: MustDisposeResource]
    internal ref struct DisposableRefStruct()
    {
        public void Dispose() { }
    }

    internal class Parent
    {
        public virtual void NotAnnotated(ref IDisposable p0, out IAsyncDisposable p1) => throw new NotImplementedException();

        public virtual void Annotated([MustDisposeResource] ref Stream p0, [MustDisposeResource] out DisposableRefStruct p1) => throw new NotImplementedException();

        public virtual void AnnotatedWithFalse([MustDisposeResource(false)] ref Stream p0, [MustDisposeResource(false)] out DisposableRefStruct p1) => throw new NotImplementedException();
    }

    internal class Child : Parent
    {
        public override void NotAnnotated(ref IDisposable p0, out IAsyncDisposable p1) => throw new NotImplementedException();

        public override void Annotated(ref Stream p0, out DisposableRefStruct p1) => throw new NotImplementedException();

        public override void AnnotatedWithFalse(ref Stream p0, out DisposableRefStruct p1) => throw new NotImplementedException();

        public void Other(ref IDisposable p0, out DisposableRefStruct p1) => throw new NotImplementedException();
    }

    internal class ParentTaskLike
    {
        public virtual void NotAnnotated(ref ValueTask<IDisposable> p0, out Task<IAsyncDisposable> p1) => throw new NotImplementedException();

        public virtual void Annotated([MustDisposeResource] ref ValueTask<Stream> p0, [MustDisposeResource] out Task<IDisposable> p1) => throw new NotImplementedException();

        public virtual void AnnotatedWithFalse([MustDisposeResource(false)] ref ValueTask<Stream> p0, [MustDisposeResource(false)] out Task<IAsyncDisposable> p1) => throw new NotImplementedException();
    }

    internal class ChildTaskLike : ParentTaskLike
    {
        public override void NotAnnotated(ref ValueTask<IDisposable> p0, out Task<IAsyncDisposable> p1) => throw new NotImplementedException();

        public override void Annotated(ref ValueTask<Stream> p0, out Task<IDisposable> p1) => throw new NotImplementedException();

        public override void AnnotatedWithFalse(ref ValueTask<Stream> p0, out Task<IAsyncDisposable> p1) => throw new NotImplementedException();

        public void Other(ref Task<IDisposable> p0, out ValueTask<Stream> p1) => throw new NotImplementedException();
    }

    internal class Other
    {
        public void InputParameters([MustDisposeResource] IDisposable p0, [MustDisposeResource] in IDisposable p1, [MustDisposeResource] ref readonly IDisposable p2) { }

        public void NonDisposable([MustDisposeResource] ref int p0, [MustDisposeResource] out int p1) => throw new NotImplementedException();
    }
}