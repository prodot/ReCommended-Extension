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
    }
}