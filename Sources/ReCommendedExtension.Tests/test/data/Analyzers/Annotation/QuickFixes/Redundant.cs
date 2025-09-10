using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test
{
    internal class BaseInterfaceImplementation : IBase
    {
        [Not{caret}Null]
        async Task<int> Redundant()
        {
            throw new NotImplementedException();
        }
    }
}