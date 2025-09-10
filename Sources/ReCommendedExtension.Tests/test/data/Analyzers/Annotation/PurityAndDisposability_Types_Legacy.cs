using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DisposableWithNoBaseTypeAnnotated
{
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    internal class Child : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord { }
}

namespace DisposableWithNearestBaseTypeAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class GrandParent : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal class Parent : GrandParent { }

    internal class Child : Parent { }

    internal record GrandParentRecord : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord { }
}

namespace DisposableWithNearestTypeAnnotated
{
    [MustDisposeResource]
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    internal class Child : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord : ParentRecord { }
}

namespace DisposableWithNearestTypeAnnotatedAgain
{
    [MustDisposeResource]
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    [MustDisposeResource]
    internal class Child : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal record ParentRecord : GrandParentRecord { }

    [MustDisposeResource]
    internal record ChildRecord : ParentRecord { }
}

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class WithoutParameters : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal class WithFalse : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal class WithTrue : IDisposable
    {
        public void Dispose() { }
    }
}

namespace NonDisposable
{
    [MustDisposeResource]
    internal class Class { }

    [MustDisposeResource(false)]
    internal record Record { }
}

namespace Structs
{
    internal struct DisposableStruct : IDisposable
    {
        public void Dispose() { }
    }

    internal record struct DisposableStructRecord : IDisposable
    {
        public void Dispose() { }
    }

    internal struct DisposableStructWithCtor : IDisposable
    {
        [MustDisposeResource]
        public DisposableStructWithCtor() { }

        public void Dispose() { }
    }

    internal record struct DisposableStructRecordWithCtor : IDisposable
    {
        [MustDisposeResource]
        public DisposableStructRecordWithCtor() { }

        public void Dispose() { }
    }

    [method: MustDisposeResource]
    internal struct DisposableStructWithPrimaryCtor() : IDisposable
    {
        public void Dispose() { }
    }

    [method: MustDisposeResource]
    internal record struct DisposableStructRecordWithPrimaryCtor() : IDisposable
    {
        public void Dispose() { }
    }

    internal struct NonDisposableStruct { }

    internal record struct NonDisposableStructRecord { }

    internal ref struct RefStruct { }

    internal ref struct RefStructWithDispose
    {
        public void Dispose() { }
    }

    internal ref struct RefStructWithDisposeAsync
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWithDisposeAndCtor
    {
        [MustDisposeResource]
        public RefStructWithDisposeAndCtor() { }

        public void Dispose() { }
    }

    internal ref struct RefStructWithDisposeAsyncAndCtor
    {
        [MustDisposeResource]
        public RefStructWithDisposeAsyncAndCtor() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [method: MustDisposeResource]
    internal ref struct RefStructWithDisposeAndPrimaryCtor()
    {
        public void Dispose() { }
    }

    [method: MustDisposeResource]
    internal ref struct RefStructWithDisposeAsyncAndAndPrimaryCtor()
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    public class PublicContainer
    {
        ref struct NestedRefStructWithDispose
        {
            public void Dispose() { }
        }
    }

    internal class InternalContainer
    {
        ref struct NestedRefStructWithDispose
        {
            public void Dispose() { }
        }
    }
}

namespace PartialTypes
{
    internal partial class DisposableClass { }

    [MustDisposeResource]
    internal partial class DisposableClass : IDisposable
    {
        public void Dispose() { }
    }

    internal partial struct DisposableStruct
    {
        [MustDisposeResource]
        public DisposableStruct(int x) { }
    }

    internal partial struct DisposableStruct : IDisposable
    {
        public void Dispose() { }
    }
}

namespace Other
{
    internal interface IInterface { }

    internal enum Enum { }

    internal delegate void Delegate();
}