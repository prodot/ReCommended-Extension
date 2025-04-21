using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal interface IBase
    {
        IEnumerable<string> Iterator(IEnumerable<string> p);

        Task<string> AsyncMethod();

        Task<string> AsyncProperty1 { get; set; }

        Task<string> AsyncProperty2 { get; }

        Task<string> this[Task<string> index] { get; set; }

        Task<string> this[Task<string> index, Task<string> index2] { get; }

        Lazy<string> LazyMethod();

        Lazy<string> LazyProperty1 { get; set; }

        Lazy<string> LazyProperty2 { get; }

        Lazy<string> this[Lazy<string> index] { get; set; }

        Lazy<string> this[Lazy<string> index, Lazy<string> index2] { get; }

        ValueTask<string> AsyncMethod2();

        ValueTask<string> AsyncProperty3 { get; set; }

        ValueTask<string> AsyncProperty4 { get; }

        ValueTask<string> this[ValueTask<string> index] { get; set; }

        ValueTask<string> this[ValueTask<string> index, ValueTask<string> index2] { get; }
    }

    internal class BaseImplementation : IBase
    {
        [ItemNotNull]
        public IEnumerable<string> Iterator([ItemNotNull] IEnumerable<string> p) => null;

        [ItemNotNull]
        public Task<string> AsyncMethod() => null;

        [ItemNotNull]
        public Task<string> AsyncProperty1 { get; set; }

        [ItemNotNull]
        public Task<string> AsyncProperty2 { get; }

        [ItemNotNull]
        Task<string> IBase.this[[ItemNotNull] Task<string> index]
        {
            get
            {
                return null;
            }
            set { }
        }

        [ItemNotNull]
        Task<string> IBase.this[[ItemNotNull] Task<string> index, [ItemNotNull] Task<string> index2] => null;

        [ItemNotNull]
        public Lazy<string> LazyMethod() => null;

        [ItemNotNull]
        public Lazy<string> LazyProperty1 { get; set; }

        [ItemNotNull]
        public Lazy<string> LazyProperty2 { get; }

        [ItemNotNull]
        Lazy<string> IBase.this[[ItemNotNull] Lazy<string> index]
        {
            get
            {
                return null;
            }
            set { }
        }

        [ItemNotNull]
        Lazy<string> IBase.this[[ItemNotNull] Lazy<string> index, [ItemNotNull] Lazy<string> index2] => null;

        [ItemNotNull]
        IEnumerable<int> IteratorWithValueTypes([ItemNotNull] IEnumerable<int> p) => null;

        [ItemNotNull]
        int[] IteratorWithValueTypes([ItemNotNull] int[] p) => null;

        [ItemNotNull]
        Task TaskProperty { get; set; }

        [ItemNotNull]
        Task<int> TaskPropertyWithValueType { get; set; }

        [ItemNotNull]
        Task this[[ItemNotNull] Task index] => null;

        [ItemNotNull]
        Task<int> this[[ItemNotNull] Task<int> index] => null;

        [ItemNotNull]
        Lazy<int> lazyField;

        [ItemNotNull]
        delegate Task<int> DelegateWithValueType([ItemNotNull] Task<int> p);

        [ItemNotNull]
        string Other([ItemNotNull] int p) => null;

        [ItemNotNull]
        IEnumerable<(int, string)> IteratorWithValueTypes2([ItemNotNull] IEnumerable<(int, string)> p) => null;

        [ItemNotNull]
        (int, string)[] IteratorWithValueTypes2([ItemNotNull] (int, string)[] p) => null;

        [ItemNotNull]
        Task<(int, string)> TaskPropertyWithValueType2 { get; set; }

        [ItemNotNull]
        Task<(int, string)> this[[ItemNotNull] Task<(int, string)> index] => null;

        [ItemNotNull]
        Lazy<(int, string)> lazyField2;

        [ItemNotNull]
        delegate Task<(int, string)> DelegateWithValueType2([ItemNotNull] Task<(int, string)> p);

        [ItemNotNull]
        string Other([ItemNotNull] (int, string) p) => null;

        [ItemNotNull]
        public ValueTask<string> AsyncMethod2() => default;

        [ItemNotNull]
        public ValueTask<string> AsyncProperty3 { get; set; }

        [ItemNotNull]
        public ValueTask<string> AsyncProperty4 { get; }

        [ItemNotNull]
        ValueTask<string> IBase.this[[ItemNotNull] ValueTask<string> index]
        {
            get
            {
                return default;
            }
            set { }
        }

        [ItemNotNull]
        ValueTask<string> IBase.this[[ItemNotNull] ValueTask<string> index, [ItemNotNull] ValueTask<string> index2] => default;

        [ItemNotNull]
        ValueTask TaskProperty2 { get; set; }

        [ItemNotNull]
        ValueTask<int> TaskPropertyWithValueType3 { get; set; }

        [ItemNotNull]
        ValueTask this[[ItemNotNull] ValueTask index] => default;

        [ItemNotNull]
        ValueTask<int> this[[ItemNotNull] ValueTask<int> index] => default;

        [ItemNotNull]
        delegate ValueTask<int> DelegateWithValueType3([ItemNotNull] ValueTask<int> p);

        [ItemNotNull]
        delegate ValueTask<(int, string)> DelegateWithValueType4([ItemNotNull] ValueTask<(int, string)> p);
    }
}