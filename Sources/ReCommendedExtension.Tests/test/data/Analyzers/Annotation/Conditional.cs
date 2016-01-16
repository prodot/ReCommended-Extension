#define CUSTOM
#define CUSTOM2

using System;
using System.Diagnostics;

namespace Test
{
    [Conditional("CUSTOM")]
    internal sealed class CustomAttribute : Attribute { }

    [Conditional("CUSTOM")]
    [Conditional("CUSTOM2")]
    internal sealed class Custom2Attribute : Attribute { }

    internal class Class
    {
#if CUSTOM
        [Custom]
        void Method() { }
#else
        [Custom]
        void Method() { }
#endif

#if CUSTOM || CUSTOM2
        [Custom2]
        void Method2() { }
#else
        [Custom2]
        void Method2() { }
#endif

    }
}