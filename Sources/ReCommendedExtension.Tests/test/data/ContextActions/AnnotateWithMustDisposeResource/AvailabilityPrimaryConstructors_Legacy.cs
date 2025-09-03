﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class Class({off}) : IDisposable
    {
        public void Dispose() { }    
    }

    [MustDisposeResource]
    internal record Record({off}) : IAsyncDisposable
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

    internal class Child({off}) : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal record ParentRecord : GrandParentRecord { }

    internal record ChildRecord({off}) : ParentRecord { }
}

namespace NonDisposable
{
    internal class Class({off}) { }

    internal record Record({off}) { }
}

namespace DisposableStructs
{
    internal struct DisposableStruct({on}) : IDisposable
    {
        public void Dispose() { }
    }

    internal record struct DisposableStructRecord({on}) : IDisposable
    {
        public void Dispose() { }
    }
}

namespace NonDisposableStructs
{
    internal struct NonDisposableStruct({off}) { }

    internal record struct NonDisposableStructRecord({off}) { }
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