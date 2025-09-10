using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class Class : IDisposable
    {
        public Cla{on}ss() { }
        public void Dispose() { }    
    }

    [MustDisposeResource]
    internal record Record : IAsyncDisposable
    {
        public Reco{on}rd() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class Class : IDisposable
    {
        public Cla{on}ss() { }
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record Record : IAsyncDisposable
    {
        public Rec{on}ord() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableWithNoBaseTypeAnnotated
{
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    internal class Child : Parent
    {
        public Chi{on}ld() { }
    }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord
    {
        public Child{on}Record() { }
    }
}

namespace DisposableWithNearestBaseTypeAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class GrandParent : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal class Parent : GrandParent { }

    internal class Child : Parent
    {
        public Ch{on}ild() { }
    }

    internal record GrandParentRecord : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord
    {
        public Child{on}Record() { }
    }
}

namespace DisposableWithNearestTypeAnnotated
{
    [MustDisposeResource]
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    internal class Child : Parent
    {
        public Chi{on}ld() { }
    }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord
    {
        public Child{on}Record() { }
    }
}

namespace NonDisposable
{
    internal class Class
    {
        public Cla{off}ss() { }
    }

    internal record Record
    {
        public Reco{off}rd() { }
    }
}

namespace DisposableStructs
{
    internal struct DisposableStruct : IDisposable
    {
        public Disposable{on}Struct() { }
        public void Dispose() { }
    }

    internal record struct DisposableStructRecord : IDisposable
    {
        public DisposableStruct{on}Record() { }
        public void Dispose() { }
    }
}

namespace NonDisposableStructs
{
    internal struct NonDisposableStruct
    {
        public NonDisposable{off}Struct() { }
    }

    internal record struct NonDisposableStructRecord
    {
        public NonDisposableStruct{off}Record() { }
    }
}

namespace RefStructs
{
    internal ref struct RefStruct
    {
        public Ref{off}Struct() { }
    }

    internal ref struct RefStructWithDispose
    {
        public RefStructWith{on}Dispose() { }
        public void Dispose() { }
    }

    internal ref struct RefStructWithDisposeAsync
    {
        public RefStructWith{on}DisposeAsync() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWithClose
{
        public RefStructWith{on}Close() { }

        [HandlesResourceDisposal]
        public void Close() { }
    }
}