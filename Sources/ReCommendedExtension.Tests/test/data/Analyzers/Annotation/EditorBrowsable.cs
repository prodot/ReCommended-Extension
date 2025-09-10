using System;
using System.ComponentModel;

namespace Test
{
    public class SomeClass
    {
        public void Deconstruct(out int x)
        {
            x = 1;
        }

        public void Deconstruct(out int x, out bool y)
        {
            x = 1;
            y = true;
        }

        public void Deconstruct<T>(out int x, out bool y, out T z)
        {
            x = 1;
            y = true;
            z = default;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Deconstruct<T, U>(out int x, out bool y, out T z, out U z2)
        {
            x = 1;
            y = true;
            z = default;
            z2 = default;
        }
    }

    public abstract class BaseClass
    {
        public abstract void Deconstruct(out int x);

        public abstract void Deconstruct(out int x, out bool y);

        [EditorBrowsable(EditorBrowsableState.Always)]
        public abstract void Deconstruct<T>(out int x, out bool y, out T z);

        [EditorBrowsable(EditorBrowsableState.Always)]
        public abstract void Deconstruct<T, U>(out int x, out bool y, out T z, out U z2);

        [EditorBrowsable(EditorBrowsableState.Always)]
        public abstract void Deconstruct<T, U>(out int x, out bool y, out T z, out U z2, out string z3);
    }

    public class DerivedClass : BaseClass
    {
        public override void Deconstruct(out int x)
        {
            x = 1;
        }

        public override void Deconstruct(out int x, out bool y)
        {
            x = 1;
            y = true;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public override void Deconstruct<T>(out int x, out bool y, out T z)
        {
            x = 1;
            y = true;
            z = default;
        }

        public override void Deconstruct<T, U>(out int x, out bool y, out T z, out U z2)
        {
            x = 1;
            y = true;
            z = default;
            z2 = default;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Deconstruct<T, U>(out int x, out bool y, out T z, out U z2, out string z3)
        {
            x = 1;
            y = true;
            z = default;
            z2 = default;
            z3 = "";
        }
    }

    public class DerivedDerivedClass : DerivedClass
    {
        public new void Deconstruct(out int x, out bool y)
        {
            x = 1;
            y = true;
        }
    }

    public interface IInterface
    {
        [EditorBrowsable(EditorBrowsableState.Always)]
        void Deconstruct(out int x, out bool y);
    }

    public class ImplementingClass : IInterface
    {
        public void Deconstruct(out int x, out bool y)
        {
            x = 1;
            y = true;
        }
    }

    public class C { }

    public static class Extensions
    {
        public static void Deconstruct(this C c, out int x)
        {
            x = 1;
        }

        public static void Deconstruct(this C c, out int x, out bool y)
        {
            x = 1;
            y = true;
        }

        public static void Deconstruct<T>(this C c, out int x, out bool y, out T z)
        {
            x = 1;
            y = true;
            z = default;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public static void Deconstruct<T, U>(this C c, out int x, out bool y, out T z, out U z2)
        {
            x = 1;
            y = true;
            z = default;
            z2 = default;
        }
    }
}

namespace PartialClasses
{
    public partial class PartialClass
    {
        public partial void Deconstruct(out int x, out bool y);

        [EditorBrowsable(EditorBrowsableState.Always)]
        public partial void Deconstruct<T>(out int x, out bool y, out T z);
    }

    public partial class PartialClass
    {
        public partial void Deconstruct(out int x, out bool y)
        {
            x = 1;
            y = true;
        }

        public partial void Deconstruct<T>(out int x, out bool y, out T z)
        {
            x = 1;
            y = true;
            z = default;
        }
    }
}