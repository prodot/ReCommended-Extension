using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NonDisposable
{
    internal class Parent
    {
        public virtual int NotAnnotated() => 0;

        [Pure]
        public virtual int AnnotatedWithPure() => 0;

        [MustUseReturnValue]
        public virtual int AnnotatedWithMustUseReturnValue() => 0;

        [MustDisposeResource]
        public virtual int AnnotatedWithMustDisposeResource() => 0;

        [MustDisposeResource(false)]
        public virtual int AnnotatedWithMustDisposeResourceFalse() => 0;

        [MustDisposeResource(true)]
        public virtual int AnnotatedWithMustDisposeResourceTrue() => 0;
    }

    internal class ChildNotAnnotated : Parent
    {
        public override int NotAnnotated() => 0;

        public override int AnnotatedWithPure() => 0;

        public override int AnnotatedWithMustUseReturnValue() => 0;

        public override int AnnotatedWithMustDisposeResource() => 0;

        public override int AnnotatedWithMustDisposeResourceFalse() => 0;

        public override int AnnotatedWithMustDisposeResourceTrue() => 0;

        public int Other() => 0;
    }

    internal class ChildAnnotatedWithPure : Parent
    {
        [Pure]
        public override int NotAnnotated() => 0;

        [Pure]
        public override int AnnotatedWithPure() => 0;

        [Pure]
        public override int AnnotatedWithMustUseReturnValue() => 0;

        [Pure]
        public override int AnnotatedWithMustDisposeResource() => 0;

        [Pure]
        public override int AnnotatedWithMustDisposeResourceFalse() => 0;

        [Pure]
        public override int AnnotatedWithMustDisposeResourceTrue() => 0;

        [Pure]
        public int Other() => 0;
    }

    internal class ChildAnnotatedWithMustUseReturnValue : Parent
    {
        [MustUseReturnValue]
        public override int NotAnnotated() => 0;

        [MustUseReturnValue]
        public override int AnnotatedWithPure() => 0;

        [MustUseReturnValue]
        public override int AnnotatedWithMustUseReturnValue() => 0;

        [MustUseReturnValue]
        public override int AnnotatedWithMustDisposeResource() => 0;

        [MustUseReturnValue]
        public override int AnnotatedWithMustDisposeResourceFalse() => 0;

        [MustUseReturnValue]
        public override int AnnotatedWithMustDisposeResourceTrue() => 0;

        [MustUseReturnValue]
        public int Other() => 0;
    }

    internal class ChildAnnotatedWithMustDisposeResource : Parent
    {
        [MustDisposeResource]
        public override int NotAnnotated() => 0;

        [MustDisposeResource]
        public override int AnnotatedWithPure() => 0;

        [MustDisposeResource]
        public override int AnnotatedWithMustUseReturnValue() => 0;

        [MustDisposeResource]
        public override int AnnotatedWithMustDisposeResource() => 0;

        [MustDisposeResource]
        public override int AnnotatedWithMustDisposeResourceFalse() => 0;

        [MustDisposeResource]
        public override int AnnotatedWithMustDisposeResourceTrue() => 0;

        [MustDisposeResource]
        public int Other() => 0;
    }

    internal class ChildAnnotatedWithMustDisposeResourceFalse : Parent
    {
        [MustDisposeResource(false)]
        public override int NotAnnotated() => 0;

        [MustDisposeResource(false)]
        public override int AnnotatedWithPure() => 0;

        [MustDisposeResource(false)]
        public override int AnnotatedWithMustUseReturnValue() => 0;

        [MustDisposeResource(false)]
        public override int AnnotatedWithMustDisposeResource() => 0;

        [MustDisposeResource(false)]
        public override int AnnotatedWithMustDisposeResourceFalse() => 0;

        [MustDisposeResource(false)]
        public override int AnnotatedWithMustDisposeResourceTrue() => 0;

        [MustDisposeResource(false)]
        public int Other() => 0;
    }

    internal class ChildAnnotatedWithMustDisposeResourceTrue : Parent
    {
        [MustDisposeResource(true)]
        public override int NotAnnotated() => 0;

        [MustDisposeResource(true)]
        public override int AnnotatedWithPure() => 0;

        [MustDisposeResource(true)]
        public override int AnnotatedWithMustUseReturnValue() => 0;

        [MustDisposeResource(true)]
        public override int AnnotatedWithMustDisposeResource() => 0;

        [MustDisposeResource(true)]
        public override int AnnotatedWithMustDisposeResourceFalse() => 0;

        [MustDisposeResource(true)]
        public override int AnnotatedWithMustDisposeResourceTrue() => 0;

        [MustDisposeResource(true)]
        public int Other() => 0;
    }

    internal class WithAnnotations
    {
        [Pure]
        public int WithPure() => 0;

        [MustUseReturnValue]
        public int WithMustUseReturnValue() => 0;

        [MustDisposeResource]
        public int WithMustDisposeResource() => 0;

        [MustDisposeResource(false)]
        public int WithMustDisposeResourceFalse() => 0;

        [MustDisposeResource(true)]
        public int WithMustDisposeResourceTrue() => 0;
    }

    internal class WithConflictingAnnotations
    {
        [Pure]
        [MustUseReturnValue]
        public int Pure_MustUseReturnValue() => 0;

        [Pure]
        [MustDisposeResource]
        public int Pure_MustDisposeResource() => 0;

        [MustUseReturnValue]
        [MustDisposeResource]
        public int MustUseReturnValue_MustDisposeResource() => 0;

        [Pure]
        [MustUseReturnValue]
        [MustDisposeResource]
        public int All() => 0;
    }
}

namespace Disposable
{
    [method: MustDisposeResource]
    internal ref struct DisposableRefStruct()
    {
        public void Dispose() { }
    }

    internal class Parent
    {
        public virtual IDisposable NotAnnotated() => throw new NotImplementedException();

        [Pure]
        public virtual IAsyncDisposable AnnotatedWithPure() => throw new NotImplementedException();

        [MustUseReturnValue]
        public virtual Stream AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustDisposeResource]
        public virtual Stream AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public virtual IAsyncDisposable AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustDisposeResource(true)]
        public virtual IAsyncDisposable AnnotatedWithMustDisposeResourceTrue() => throw new NotImplementedException();
    }

    internal class ChildNotAnnotated : Parent
    {
        public override IDisposable NotAnnotated() => throw new NotImplementedException();

        public override IAsyncDisposable AnnotatedWithPure() => throw new NotImplementedException();

        public override Stream AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        public override Stream AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        public override IAsyncDisposable AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        public override IAsyncDisposable AnnotatedWithMustDisposeResourceTrue() => throw new NotImplementedException();

        public DisposableRefStruct Other() => throw new NotImplementedException();
    }

    internal class ChildAnnotatedWithPure : Parent
    {
        [Pure]
        public override IDisposable NotAnnotated() => throw new NotImplementedException();

        [Pure]
        public override IAsyncDisposable AnnotatedWithPure() => throw new NotImplementedException();

        [Pure]
        public override Stream AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [Pure]
        public override Stream AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [Pure]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [Pure]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceTrue() => throw new NotImplementedException();

        [Pure]
        public DisposableRefStruct Other() => throw new NotImplementedException();
    }

    internal class ChildAnnotatedWithMustUseReturnValue : Parent
    {
        [MustUseReturnValue]
        public override IDisposable NotAnnotated() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override IAsyncDisposable AnnotatedWithPure() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override Stream AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override Stream AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceTrue() => throw new NotImplementedException();

        [MustUseReturnValue]
        public DisposableRefStruct Other() => throw new NotImplementedException();
    }

    internal class ChildAnnotatedWithMustDisposeResource : Parent
    {
        [MustDisposeResource]
        public override IDisposable NotAnnotated() => throw new NotImplementedException();

        [MustDisposeResource]
        public override IAsyncDisposable AnnotatedWithPure() => throw new NotImplementedException();

        [MustDisposeResource]
        public override Stream AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustDisposeResource]
        public override Stream AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustDisposeResource]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustDisposeResource]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceTrue() => throw new NotImplementedException();

        [MustDisposeResource]
        public DisposableRefStruct Other() => throw new NotImplementedException();
    }

    internal class ChildAnnotatedWithMustDisposeResourceFalse : Parent
    {
        [MustDisposeResource(false)]
        public override IDisposable NotAnnotated() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override IAsyncDisposable AnnotatedWithPure() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override Stream AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override Stream AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceTrue() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public DisposableRefStruct Other() => throw new NotImplementedException();
    }

    internal class ChildAnnotatedWithMustDisposeResourceTrue : Parent
    {
        [MustDisposeResource(true)]
        public override IDisposable NotAnnotated() => throw new NotImplementedException();

        [MustDisposeResource(true)]
        public override IAsyncDisposable AnnotatedWithPure() => throw new NotImplementedException();

        [MustDisposeResource(true)]
        public override Stream AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustDisposeResource(true)]
        public override Stream AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustDisposeResource(true)]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustDisposeResource(true)]
        public override IAsyncDisposable AnnotatedWithMustDisposeResourceTrue() => throw new NotImplementedException();

        [MustDisposeResource(true)]
        public DisposableRefStruct Other() => throw new NotImplementedException();
    }

    internal class ParentTaskLike
    {
        public virtual Task<IDisposable> NotAnnotated() => throw new NotImplementedException();

        [Pure]
        public virtual Task<IAsyncDisposable> AnnotatedWithPure() => throw new NotImplementedException();

        [MustUseReturnValue]
        public virtual Task<Stream> AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustDisposeResource]
        public virtual ValueTask<Stream> AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public virtual ValueTask<IAsyncDisposable> AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();
    }

    internal class ChildTaskLikeNotAnnotated : ParentTaskLike
    {
        public override Task<IDisposable> NotAnnotated() => throw new NotImplementedException();

        public override Task<IAsyncDisposable> AnnotatedWithPure() => throw new NotImplementedException();

        public override Task<Stream> AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        public override ValueTask<Stream> AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        public override ValueTask<IAsyncDisposable> AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        public ValueTask<IDisposable> Other() => throw new NotImplementedException();
    }

    internal class ChildTaskLikeAnnotatedWithPure : ParentTaskLike
    {
        [Pure]
        public override Task<IDisposable> NotAnnotated() => throw new NotImplementedException();

        [Pure]
        public override Task<IAsyncDisposable> AnnotatedWithPure() => throw new NotImplementedException();

        [Pure]
        public override Task<Stream> AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [Pure]
        public override ValueTask<Stream> AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [Pure]
        public override ValueTask<IAsyncDisposable> AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [Pure]
        public ValueTask<IDisposable> Other() => throw new NotImplementedException();
    }

    internal class ChildTaskLikeAnnotatedWithMustUseReturnValue : ParentTaskLike
    {
        [MustUseReturnValue]
        public override Task<IDisposable> NotAnnotated() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override Task<IAsyncDisposable> AnnotatedWithPure() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override Task<Stream> AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override ValueTask<Stream> AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustUseReturnValue]
        public override ValueTask<IAsyncDisposable> AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustUseReturnValue]
        public ValueTask<IDisposable> Other() => throw new NotImplementedException();
    }

    internal class ChildTaskLikeAnnotatedWithMustDisposeResource : ParentTaskLike
    {
        [MustDisposeResource]
        public override Task<IDisposable> NotAnnotated() => throw new NotImplementedException();

        [MustDisposeResource]
        public override Task<IAsyncDisposable> AnnotatedWithPure() => throw new NotImplementedException();

        [MustDisposeResource]
        public override Task<Stream> AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustDisposeResource]
        public override ValueTask<Stream> AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustDisposeResource]
        public override ValueTask<IAsyncDisposable> AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustDisposeResource]
        public ValueTask<IDisposable> Other() => throw new NotImplementedException();
    }

    internal class ChildTaskLikeAnnotatedWithMustDisposeResourceFalse : ParentTaskLike
    {
        [MustDisposeResource(false)]
        public override Task<IDisposable> NotAnnotated() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override Task<IAsyncDisposable> AnnotatedWithPure() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override Task<Stream> AnnotatedWithMustUseReturnValue() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override ValueTask<Stream> AnnotatedWithMustDisposeResource() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public override ValueTask<IAsyncDisposable> AnnotatedWithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public ValueTask<IDisposable> Other() => throw new NotImplementedException();
    }

    internal class WithAnnotations
    {
        [Pure]
        public IDisposable WithPure() => throw new NotImplementedException();

        [MustUseReturnValue]
        public IDisposable WithMustUseReturnValue() => throw new NotImplementedException();

        [MustDisposeResource]
        public IDisposable WithMustDisposeResource() => throw new NotImplementedException();

        [MustDisposeResource(false)]
        public IDisposable WithMustDisposeResourceFalse() => throw new NotImplementedException();

        [MustDisposeResource(true)]
        public IDisposable WithMustDisposeResourceTrue() => throw new NotImplementedException();
    }

    internal class WithConflictingAnnotations
    {
        [Pure]
        [MustUseReturnValue]
        public IDisposable Pure_MustUseReturnValue() => throw new NotImplementedException();

        [Pure]
        [MustDisposeResource]
        public IDisposable Pure_MustDisposeResource() => throw new NotImplementedException();

        [MustUseReturnValue]
        [MustDisposeResource]
        public IDisposable MustUseReturnValue_MustDisposeResource() => throw new NotImplementedException();

        [Pure]
        [MustUseReturnValue]
        [MustDisposeResource]
        public IDisposable All() => throw new NotImplementedException();
    }

    internal class WithConflictingAnnotationsTaskLike
    {
        [Pure]
        [MustUseReturnValue]
        public Task<IDisposable> Pure_MustUseReturnValue() => throw new NotImplementedException();

        [Pure]
        [MustDisposeResource]
        public ValueTask<IDisposable> Pure_MustDisposeResource() => throw new NotImplementedException();

        [MustUseReturnValue]
        [MustDisposeResource]
        public Task<IDisposable> MustUseReturnValue_MustDisposeResource() => throw new NotImplementedException();

        [Pure]
        [MustUseReturnValue]
        [MustDisposeResource]
        public ValueTask<IDisposable> All() => throw new NotImplementedException();
    }

    internal class NullableTypes
    {
        [MustDisposeResource]
        public class Class : IDisposable
        {
            public void Dispose() { }
        }

        [method: MustDisposeResource]
        public struct Struct() : IDisposable
        {
            public void Dispose() { }
        }

        public Class? NullableClass() => throw new NotImplementedException();

        public Struct? NullableStruct() => throw new NotImplementedException();
    }
}