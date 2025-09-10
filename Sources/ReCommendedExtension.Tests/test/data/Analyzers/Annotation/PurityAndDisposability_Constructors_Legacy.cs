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
        public Child() { }
    }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord
    {
        public ChildRecord() { }
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
        public Child() { }
    }

    internal record GrandParentRecord : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord
    {
        public ChildRecord() { }
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
        public ChildRecord() { }
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
}

namespace DisposableStructs
{
    internal struct DisposableStruct : IDisposable
    {
        public DisposableStruct() { }
        public void Dispose() { }
    }

    internal record struct DisposableStructRecord : IDisposable
    {
        public DisposableStructRecord() { }
        public void Dispose() { }
    }
}

namespace NonDisposableStructs
{
    internal struct NonDisposableStruct
    {
        [MustDisposeResource]
        public NonDisposableStruct() { }
    }

    internal record struct NonDisposableStructRecord
    {
        [MustDisposeResource(false)]
        public NonDisposableStructRecord() { }
    }
}

namespace RefStructs
{
    internal ref struct RefStruct
    {
        [MustDisposeResource]
        public RefStruct() { }
    }

    internal ref struct RefStructWithDispose
    {
        public RefStructWithDispose() { }
        public void Dispose() { }
    }

    internal ref struct RefStructWithDisposeAsync
    {
        public RefStructWithDisposeAsync() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWithClose
    {
        public RefStructWithClose() { }

        [HandlesResourceDisposal]
        public void Close() { }
    }
}