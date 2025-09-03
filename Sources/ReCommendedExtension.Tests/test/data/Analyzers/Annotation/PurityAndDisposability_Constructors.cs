using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class Class : IDisposable
    {
        public Class() { }
        public void Dispose() { }    
    }

    [MustDisposeResource]
    internal record Record : IAsyncDisposable
    {
        public Record() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource]
    internal struct Struct : IDisposable
    {
        public Struct() { }
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record struct RecordStruct : IAsyncDisposable
    {
        public RecordStruct() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableAnnotatedAgain
{
    [MustDisposeResource]
    internal class Class : IDisposable
    {
        [MustDisposeResource]
        public Class() { }

        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record Record : IAsyncDisposable
    {
        [MustDisposeResource]
        public Record() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource]
    internal struct Struct : IDisposable
    {
        [MustDisposeResource]
        public Struct() { }

        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record struct RecordStruct : IAsyncDisposable
    {
        [MustDisposeResource]
        public RecordStruct() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class Class : IDisposable
    {
        public Class() { }
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record Record : IAsyncDisposable
    {
        public Record() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal struct Struct : IDisposable
    {
        public Struct() { }
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record struct RecordStruct : IAsyncDisposable
    {
        public RecordStruct() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableAnnotatedWithFalseAgain
{
    [MustDisposeResource(false)]
    internal class Class : IDisposable
    {
        [MustDisposeResource(false)]
        public Class() { }

        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record Record : IAsyncDisposable
    {
        [MustDisposeResource(false)]
        public Record() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal struct Struct : IDisposable
    {
        [MustDisposeResource(false)]
        public Struct() { }

        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal record struct RecordStruct : IAsyncDisposable
    {
        [MustDisposeResource(false)]
        public RecordStruct() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace DisposableWithNearestTypeAnnotatedAgain
{
    [MustDisposeResource]
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    internal class Child : Parent
    {
        [MustDisposeResource]
        public Child() { }
    }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord
    {
        [MustDisposeResource]
        public ChildRecord() { }
    }
}

namespace NonDisposable
{
    internal class Class
    {
        [MustDisposeResource]
        public Class() { }
    }

    internal record Record
    {
        [MustDisposeResource(false)]
        public Record() { }
    }

    internal struct Struct
    {
        [MustDisposeResource]
        public Struct() { }
    }

    internal record struct RecordStruct
    {
        [MustDisposeResource(false)]
        public RecordStruct() { }
    }
}

namespace RefStructs
{
    internal ref struct RefStruct
    {
        [MustDisposeResource]
        public RefStruct() { }
    }
}