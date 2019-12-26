using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Test
{
    class Sample
    {
        [NotNull]
        [ItemNotNull]
        List<string> notNullables;

        [CanBeNull]
        [ItemCanBeNull]
        List<string> nullables;
    }
}