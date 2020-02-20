using System;

namespace ReCommendedExtension.Tests.test.data
{
    class Generic<T>
    {
        public T Value { get; }
    }

    class Worx
    {
        void NonNullable(Generic<string> g)
        {
            Console.WriteLine(g.Value.Length);
        }

        void Nullable(Generic<string?> g)
        {
            Console.WriteLine(g.Value.Length);
        }

        void NullableSuppressed(Generic<string?> g)
        {
            Console.WriteLine(g.Value!.Length);
        }
    }
}