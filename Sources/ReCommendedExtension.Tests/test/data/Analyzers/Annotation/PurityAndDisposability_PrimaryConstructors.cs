using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class Class() : IDisposable
    {
        public void Dispose() { }    
    }

    [MustDisposeResource]
    internal record Record() : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource]
    internal struct Struct() : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record struct RecordStruct() : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableAnnotatedAgain
{
    [MustDisposeResource]
    [method: MustDisposeResource]
    internal class Class() : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    [method: MustDisposeResource]
    internal record Record() : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource]
    [method: MustDisposeResource]
    internal struct Struct() : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    [method: MustDisposeResource]
    internal record struct RecordStruct() : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class Class() : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record Record() : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal struct Struct() : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record struct RecordStruct() : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableAnnotatedWithFalseAgain
{
    [MustDisposeResource(false)]
    [method: MustDisposeResource(false)]
    internal class Class() : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    [method: MustDisposeResource(false)]
    internal record Record() : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    [method: MustDisposeResource(false)]
    internal struct Struct() : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    [method: MustDisposeResource(false)]
    internal record struct RecordStruct() : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
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

    internal class Child() : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord() : ParentRecord { }
}

namespace DisposableWithNearestTypeAnnotatedAgain
{
    [MustDisposeResource]
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    [method: MustDisposeResource]
    internal class Child() : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record ParentRecord : GrandParentRecord { }

    [method: MustDisposeResource]
    internal record ChildRecord() : ParentRecord { }
}

namespace NonDisposable
{
    [method: MustDisposeResource]
    internal class Class() { }

    [method: MustDisposeResource(false)]
    internal record Record() { }

    [method: MustDisposeResource]
    internal struct Struct() { }

    [method: MustDisposeResource(false)]
    internal record struct RecordStruct() { }
}

namespace NonDisposableStructs
{
    [method: MustDisposeResource]
    internal struct NonDisposableStruct() { }

    [method: MustDisposeResource(false)]
    internal record struct NonDisposableStructRecord() { }
}

namespace RefStructs
{
    [method: MustDisposeResource]
    internal ref struct RefStruct() { }
}