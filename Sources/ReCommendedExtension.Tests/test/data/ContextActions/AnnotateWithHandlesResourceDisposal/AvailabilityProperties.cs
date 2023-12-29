using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class TestClass
    {
        public IDisposable Disposab{on}le { get; set; }

        int nonDisposabl{off}e { get; set; }

        [HandlesResourceDisposal]
        public virtual IDisposable Disposable{off}Annotated { get; set; }
    }

    internal class DerivedClass : TestClass
    {
        public override IDisposable Disposable{off}Annotated { get; set; }
    }
}