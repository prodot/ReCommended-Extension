using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class TestClass
    {
        IDisposable disposab{on}le;

        int non{off}Disposable;

        [HandlesResourceDisposal]
        IDisposable disposable{off}Annotated;
    }
}