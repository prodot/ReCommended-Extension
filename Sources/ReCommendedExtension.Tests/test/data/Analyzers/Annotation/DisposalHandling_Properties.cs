using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class TestClass
    {
        [HandlesResourceDisposal]
        public IDisposable Disposable { get; set; }

        [HandlesResourceDisposal]
        int nonDisposable { get; set; }

        [HandlesResourceDisposal]
        public virtual IDisposable DisposableAnnotated { get; set; }
    }

    internal class DerivedClass : TestClass
    {
        [HandlesResourceDisposal]
        public override IDisposable DisposableAnnotated { get; set; }
    }
}