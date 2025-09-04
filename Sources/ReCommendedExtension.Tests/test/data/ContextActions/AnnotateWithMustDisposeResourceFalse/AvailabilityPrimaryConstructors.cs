using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class Class({on}) : IDisposable
    {
        public void Dispose() { }    
    }

    [MustDisposeResource]
    internal record Record({on}) : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource]
    internal struct Struct({on}) : IDisposable
    {
        public void Dispose() { }    
    }

    [MustDisposeResource]
    internal record struct RecordStruct({on}) : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class Class({on}) : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record Record({on}) : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal struct Struct({on}) : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record struct RecordStruct({on}) : IAsyncDisposable
    {
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

    internal class Child({on}) : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord({on}) : ParentRecord { }
}

namespace DisposableWithNearestBaseTypeAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class GrandParent : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal class Parent : GrandParent { }

    internal class Child({on}) : Parent { }

    internal record GrandParentRecord : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord({on}) : ParentRecord { }
}

namespace DisposableWithNearestTypeAnnotated
{
    [MustDisposeResource]
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    internal class Child({on}) : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord({on}) : ParentRecord { }
}

namespace NonDisposable
{
    internal class Class({off}) { }

    internal record Record({off}) { }

    internal struct Struct({off}) { }

    internal record struct RecordStruct({off}) { }
}

namespace RefStructs
{
    internal ref struct RefStruct({off}) { }

    internal ref struct RefStructWithDispose({on})
    {        
        public void Dispose() { }
    }

    internal ref struct RefStructWithDisposeAsync({on})
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWithClose({on})
    {
        [HandlesResourceDisposal]
        public void Close() { }
    }
}

namespace AnnotatedPrimaryConstructor
{
    [method: MustDisposeResource]
    internal struct Struct({on}) : IDisposable
    {
        public void Dispose() { }
    }

    [method: MustDisposeResource(false)]
    internal record struct recordStruct({off}) : IDisposable
    {
        public void Dispose() { }
    }
}