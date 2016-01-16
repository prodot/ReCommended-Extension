using System.Diagnostics;
using JetBrains.Annotations;

namespace Test
{
    internal static class Availability
    {
        static void Method(string one)
        {
            var length = one.AssertN{on}otNull().Length;
        }

        [DebuggerStepThrough]
        [NotNull]
        static T AssertNotNull<T>(this T value) where T : class
        {
            Debug.Assert(value != null);

            return value;
        }
    }
}