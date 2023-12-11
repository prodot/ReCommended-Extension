using System;
using System.Threading.Tasks;

namespace Types
{
    class NonDisposabl{off}e
    {
        public NonDisposabl{off}e() { }
    }

    class Disposable{on}Class : IDisposable
    {
        public Disposable{on}Class() { }

        public void Dispose() { }
    }

    class Disposable{on}BaseClass : IDisposable
    {
        public Disposable{on}BaseClass() { }

        public void Dispose() { }
    }

    class Disposable{on}DerivedClass : DisposableBaseClass
    {
        public Disposable{on}DerivedClass() { }
    }

    class Disposable{on}BaseClass2 : IAsyncDisposable
    {
        public Disposable{on}BaseClass2() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    class Disposable{on}DerivedClass2 : DisposableBaseClass2
    {
        public Disposable{on}DerivedClass2() { }
    }

    interface IDisposable{off}Ex : IDisposable { }

    record Disposable{on}Record : IDisposable
    {
        public Disposable{on}Record() { }

        public void Dispose() { }
    }

    record DisposableRecord{on}2 : IDisposableEx
    {
        public Disposable{on}Record2() { }

        public void Dispose() { }
    }

    interface IAsyncDisposable{off}Ex : IAsyncDisposable { }

    record DisposableBase{on}Record : IAsyncDisposableEx
    {
        public DisposableBase{on}Record() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    record DisposableDerived{on}Record : DisposableBaseRecord
    {
        public DisposableDerived{on}Record() { }
    }

    record struct Disposable{off}RecordStruct : IDisposable
    {
        public Disposable{off}RecordStruct() { }

        public void Dispose() { }
    }

    struct Disposable{off}Struct : IDisposable
    {
        public Disposable{off}Struct(int x) { }

        public void Dispose() { }
    }
}