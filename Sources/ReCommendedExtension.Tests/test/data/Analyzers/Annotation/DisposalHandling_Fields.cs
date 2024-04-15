using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class TestClass
    {
        [HandlesResourceDisposal]
        IDisposable disposable;

        [HandlesResourceDisposal]
        int nonDisposable;
    }
}