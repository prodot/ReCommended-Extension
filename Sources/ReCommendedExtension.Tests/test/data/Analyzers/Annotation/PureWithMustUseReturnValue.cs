using JetBrains.Annotations;

namespace Test
{
    internal class Class
    {
        int None() => 0;

        [Pure]
        int Pure() => 0;

        [Pure]
        int MustUseReturnValue() => 0;

        [Pure]
        [MustUseReturnValue]
        int Both() => 0;
    }
}