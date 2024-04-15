using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class TestClass
    {
        public void Disposable(
            IDisposable p{on}0, 
            in IDisposable p{on}1, 
            ref readonly IDisposable p{on}2, 
            ref IDisposable p{on}3, 
            out IDisposable p{off}4) => throw new NotImplementedException();

        public void NonDisposable(
            int p{off}0,
            in int p{off}1,
            ref readonly int p{off}2,
            ref int p{off}3,
            out int p{off}4) => throw new NotImplementedException();

        public void DisposableAnnotated(
            [HandlesResourceDisposal] IDisposable p{off}0,
            [HandlesResourceDisposal] in IDisposable p{off}1,
            [HandlesResourceDisposal] ref readonly IDisposable p{off}2,
            [HandlesResourceDisposal] ref IDisposable p{off}3,
            out IDisposable p{off}4) => throw new NotImplementedException();
    }

    internal class BaseClass
    {
        public virtual void Method([HandlesResourceDisposal] IDisposable p{off}0) { }
    }

    internal class DerivedClass : BaseClass
    {
        public override void Method(IDisposable p{off}0) { }
    }
}