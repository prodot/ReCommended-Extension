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
}

namespace DisposableWithNoBaseTypeAnnotated
{
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

    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord() : ParentRecord { }
}

namespace DisposableWithNearestBaseTypeAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class GrandParent : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal class Parent : GrandParent { }

    internal class Child() : Parent { }

    internal record GrandParentRecord : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord() : ParentRecord { }
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
}

namespace DisposableStructs
{
    internal struct DisposableStruct() : IDisposable
    {
        public void Dispose() { }
    }

    internal record struct DisposableStructRecord() : IDisposable
    {
        public void Dispose() { }
    }
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

    internal ref struct RefStructWithDispose()
    {
        public void Dispose() { }
    }

    internal ref struct RefStructWithDisposeAsync()
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWithClose()
    {
        [HandlesResourceDisposal]
        public void Close() { }
    }
}