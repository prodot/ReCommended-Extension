using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class Class : IDisposable
    {
        public Cla{off}ss() { }
        public void Dispose() { }    
    }

    [MustDisposeResource]
    internal record Record : IAsyncDisposable
    {
        public Reco{off}rd() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource]
    internal struct Struct : IDisposable
    {
        public Str{off}uct() { }
        public void Dispose() { }    
    }

    [MustDisposeResource]
    internal record struct RecordStruct : IAsyncDisposable
    {
        public Reco{off}rdStruct() { }
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

    [MustDisposeResource(false)]
    internal struct Struct : IDisposable
    {
        public Str{on}uct() { }
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record struct RecordStruct : IAsyncDisposable
    {
        public Rec{on}ordStruct() { }
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
        public Chi{off}ld() { }
    }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord
    {
        public Child{off}Record() { }
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
        public Rec{off}ord() { }
    }

    internal struct Struct
    {
        public Stru{off}ct() { }
    }

    internal record struct RecordStruct
    {
        public Rec{off}ordStruct() { }
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