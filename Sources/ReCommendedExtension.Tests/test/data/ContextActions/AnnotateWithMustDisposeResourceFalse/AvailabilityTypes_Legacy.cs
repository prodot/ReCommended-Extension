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

    internal class Chi{on}ld : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    internal record ParentRecord : GrandParentRecord { }

    internal record Child{on}Record : ParentRecord { }
}

namespace DisposableWithNearestBaseTypeAnnotatedWithFalse
{
    [MustDisposeResource(false)]
    internal class GrandParent : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal class Parent : GrandParent { }

    internal class Chi{on}ld : Parent { }

    internal record GrandParentRecord : IAsyncDisposable
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [MustDisposeResource(false)]
    internal record ParentRecord : GrandParentRecord { }

    internal record Child{on}Record : ParentRecord { }
}

namespace DisposableWithNearestTypeAnnotated
{
    [MustDisposeResource]
    internal class GrandParent : IDisposable
    {
        public void Dispose() { }
    }

    internal class Parent : GrandParent { }

    internal class Chil{on}d : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal record ParentRecord : GrandParentRecord { }

    internal record Child{on}Record : ParentRecord { }
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
    internal class Chil{on}d : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal record ParentRecord : GrandParentRecord { }

    [MustDisposeResource]
    internal record Child{on}Record : ParentRecord { }
}

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class Without{on}Parameters : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal class With{off}False : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal class With{on}True : IDisposable
    {
        public void Dispose() { }
    }
}

namespace NonDisposable
{
    internal class Cl{off}ass { }

    internal record Re{off}cord { }
}

namespace Structs
{
    internal struct Disposable{off}Struct : IDisposable
    {
        public void Dispose() { }
    }

    internal record struct DisposableStruct{off}Record : IDisposable
    {
        public void Dispose() { }
    }

    internal struct DisposableStruct{off}WithCtor : IDisposable
    {
        public DisposableStructWithCtor() { }
        public void Dispose() { }
    }

    internal record struct DisposableStruct{off}RecordWithCtor : IDisposable
    {
        public DisposableStructRecordWithCtor() { }
        public void Dispose() { }
    }

    internal struct DisposableStructWith{off}PrimaryCtor() : IDisposable
    {
        public void Dispose() { }
    }

    internal record struct DisposableStructRecord{off}WithPrimaryCtor() : IDisposable
    {
        public void Dispose() { }
    }

    internal struct NonDisposable{off}Struct { }

    internal record struct NonDisposableStruct{off}Record { }

    internal ref struct Ref{off}Struct { }

    internal ref struct RefStruct{off}WithDispose
    {
        public void Dispose() { }
    }

    internal ref struct RefStructWith{off}DisposeAsync
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWith{off}DisposeAndCtor
    {
        public RefStructWithDisposeAndCtor() { }
        public void Dispose() { }
    }

    internal ref struct RefStructWithDispose{off}AsyncAndCtor
    {
        public RefStructWithDisposeAsyncAndCtor() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWithDisposeAnd{off}PrimaryCtor()
    {
        public void Dispose() { }
    }

    internal ref struct RefStructWithDisposeAsync{off}AndAndPrimaryCtor()
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

namespace Other
{
    internal interface IInterfac{off}e { }

    internal enum En{off}um { }

    internal delegate void Dele{off}gate();
}