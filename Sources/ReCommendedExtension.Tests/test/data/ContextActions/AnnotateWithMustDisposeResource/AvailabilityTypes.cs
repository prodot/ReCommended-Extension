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

    internal class Chil{off}d : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal record ParentRecord : GrandParentRecord { }

    internal record Child{off}Record : ParentRecord { }
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
    internal class Chil{off}d : Parent { }

    internal record GrandParentRecord : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal record ParentRecord : GrandParentRecord { }

    [MustDisposeResource]
    internal record Child{off}Record : ParentRecord { }
}

namespace DisposableAnnotated
{
    [MustDisposeResource]
    internal class Without{off}Parameters : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal class With{on}False : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal class With{off}True : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource]
    internal struct SWithout{off}Parameters : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(false)]
    internal struct SWith{on}False : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal struct SWith{off}True : IDisposable
    {
        public void Dispose() { }
    }
}

namespace NonDisposable
{
    internal class Cl{off}ass { }

    internal record Re{off}cord { }

    internal struct Str{off}uct { }

    internal record struct Re{off}cordStruct { }
}

namespace Structs
{
    internal struct Disposable{on}Struct : IDisposable
    {
        public void Dispose() { }
    }

    internal record struct DisposableStruct{on}Record : IDisposable
    {
        public void Dispose() { }
    }

    internal struct DisposableStruct{on}WithCtor : IDisposable
    {
        public DisposableStructWithCtor() { }
        public void Dispose() { }
    }

    internal record struct DisposableStruct{on}RecordWithCtor : IDisposable
    {
        public DisposableStructRecordWithCtor() { }
        public void Dispose() { }
    }

    internal struct DisposableStructWith{on}PrimaryCtor() : IDisposable
    {
        public void Dispose() { }
    }

    internal record struct DisposableStructRecord{on}WithPrimaryCtor() : IDisposable
    {
        public void Dispose() { }
    }

    internal struct NonDisposable{off}Struct { }

    internal record struct NonDisposableStruct{off}Record { }

    internal ref struct Ref{off}Struct { }

    internal ref struct RefStruct{on}WithDispose
    {
        public void Dispose() { }
    }

    internal ref struct RefStructWith{on}DisposeAsync
    {
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWith{on}DisposeAndCtor
    {
        public RefStructWithDisposeAndCtor() { }
        public void Dispose() { }
    }

    internal ref struct RefStructWithDispose{on}AsyncAndCtor
    {
        public RefStructWithDisposeAsyncAndCtor() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    internal ref struct RefStructWithDisposeAnd{on}PrimaryCtor()
    {
        public void Dispose() { }
    }

    internal ref struct RefStructWithDisposeAsync{on}AndAndPrimaryCtor()
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