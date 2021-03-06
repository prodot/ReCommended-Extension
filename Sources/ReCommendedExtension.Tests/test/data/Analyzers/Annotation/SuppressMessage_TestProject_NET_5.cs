﻿using System.Diagnostics.CodeAnalysis;

namespace Test
{
    [SuppressMessage("C", "Id")]
    internal class SuppressMessage
    {
        [SuppressMessage("C", "Id")]
        void Foo() { }

        [SuppressMessage("C", "Id", Justification = "")]
        void Foo2() { }

        [SuppressMessage("C", "Id", Justification = "Justification")]
        void Foo3() { }
    }

    internal class ExcludeFromCodeCoverage
    {
        [ExcludeFromCodeCoverage]
        void Foo() { }

        [ExcludeFromCodeCoverage(Justification = "")]
        void Foo2() { }

        [ExcludeFromCodeCoverage(Justification = "Justification")]
        void Foo3() { }
    }
}