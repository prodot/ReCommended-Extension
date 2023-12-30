using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class TestClass
    {
        public void Disposable(
            [HandlesResourceDisposal] IDisposable p0,
            [HandlesResourceDisposal] in IDisposable p1,
            [HandlesResourceDisposal] ref readonly IDisposable p2,
            [MustDisposeResource][HandlesResourceDisposal] ref IDisposable p3,
            [MustDisposeResource][HandlesResourceDisposal] out IDisposable p4) => throw new NotImplementedException();

        public void NonDisposable(
            [HandlesResourceDisposal] int p0,
            [HandlesResourceDisposal] in int p1,
            [HandlesResourceDisposal] ref readonly int p2,
            [HandlesResourceDisposal] ref int p3,
            [HandlesResourceDisposal] out int p4) => throw new NotImplementedException();
    }

    internal class BaseClass
    {
        public virtual void Method([HandlesResourceDisposal] IDisposable p0) { }
    }

    internal class DerivedClass : BaseClass
    {
        public override void Method([HandlesResourceDisposal] IDisposable p0) { }
    }
}