#define CUSTOM
#undef DEBUG
#define CUSTOM2

using System.Diagnostics;

namespace Test
{
    internal class Class
    {
        [Conditional("CUSTOM")]
        static void Method() { }

        [Conditional("CUSTOM")]
        [Conditional("CUSTOM2")]
        [Conditional("CUSTOM3")]
        static void Method2() { }

        static void Method3() { }

        void Foo()
        {
            Method();

            Method2();

            Method3();
        }
    }
}