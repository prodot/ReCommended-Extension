using System;
using System.ComponentModel;

namespace Test
{
    public class SomeClass
    {
        public void Decons{off}truct(out int x)
        {
            x = 1;
        }

        public void Deconstruc{on}t(out int x, out bool y)
        {
            x = 1;
            y = true;
        }

        public void De{on}construct<T>(out int x, out bool y, out T z)
        {
            x = 1;
            y = true;
            z = default;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Deconstr{off}uct<T, U>(out int x, out bool y, out T z, out U z2)
        {
            x = 1;
            y = true;
            z = default;
            z2 = default;
        }
    }

    public class C { }

    public static class Extensions
    {
        public static void Decon{off}struct(this C c, out int x)
        {
            x = 1;
        }

        public static void Decons{on}truct(this C c, out int x, out bool y)
        {
            x = 1;
            y = true;
        }

        public static void Deco{on}nstruct<T>(this C c, out int x, out bool y, out T z)
        {
            x = 1;
            y = true;
            z = default;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static void Decons{off}truct<T, U>(this C c, out int x, out bool y, out T z, out U z2)
        {
            x = 1;
            y = true;
            z = default;
            z2 = default;
        }
    }
}