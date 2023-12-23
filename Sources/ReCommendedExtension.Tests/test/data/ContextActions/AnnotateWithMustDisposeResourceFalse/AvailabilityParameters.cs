using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal ref struct DisposableRefStruct
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
        public override void NotAnnotated(ref IDisposable p{on}0, out IAsyncDisposable p{on}1) => throw new NotImplementedException();

        public override void Annotated(ref Stream p{on}0, out DisposableRefStruct p{on}1) => throw new NotImplementedException();

        public override void AnnotatedWithFalse(ref Stream p{on}0, out DisposableRefStruct p{on}1) => throw new NotImplementedException();

        public void Other(ref IDisposable p{on}0, out DisposableRefStruct p{on}1) => throw new NotImplementedException();
    }

    internal class ParentTaskLike
    {
        public virtual void NotAnnotated(ref ValueTask<IDisposable> p0, out Task<IAsyncDisposable> p1) => throw new NotImplementedException();

        public virtual void Annotated([MustDisposeResource] ref ValueTask<Stream> p0, [MustDisposeResource] out Task<IDisposable> p1) => throw new NotImplementedException();

        public virtual void AnnotatedWithFalse([MustDisposeResource(false)] ref ValueTask<Stream> p0, [MustDisposeResource(false)] out Task<IAsyncDisposable> p1) => throw new NotImplementedException();
    }

    internal class ChildTaskLike : ParentTaskLike
    {
        public override void NotAnnotated(ref ValueTask<IDisposable> p{off}0, out Task<IAsyncDisposable> p{off}1) => throw new NotImplementedException();

        public override void Annotated(ref ValueTask<Stream> p{off}0, out Task<IDisposable> p{off}1) => throw new NotImplementedException();

        public override void AnnotatedWithFalse(ref ValueTask<Stream> p{off}0, out Task<IAsyncDisposable> p{off}1) => throw new NotImplementedException();

        public void Other(ref Task<IDisposable> p{off}0, out ValueTask<Stream> p{off}1) => throw new NotImplementedException();
    }

    internal class Other
    {
        public void InputParameters(IDisposable p{off}0, in IDisposable p{off}1, ref readonly IDisposable p{off}2) { }

        public void NonDisposable(ref int p{off}0, out int p{off}1) => throw new NotImplementedException();
    }
}