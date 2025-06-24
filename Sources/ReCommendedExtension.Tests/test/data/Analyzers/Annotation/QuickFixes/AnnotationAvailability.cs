using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal interface IBase
    {
        string NotAllowed();
    }

    internal class BaseInterfaceImplementation : IBase
    {
        [NotNull]
        public string NotAllowed() => "";

        [Pure]
        [MustUseReturnValue]
        public int Conflicting() => 0;

        [NotNull]
        async Task<int> Redundant()
        {
            throw new NotImplementedException();
        }
    }
}