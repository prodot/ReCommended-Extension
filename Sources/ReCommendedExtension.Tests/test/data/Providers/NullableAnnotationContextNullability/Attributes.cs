using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ReCommendedExtension.Tests.test.data
{
    class Preconditions
    {
        void Method([AllowNull] string one, [AllowNull] in string two, [DisallowNull] string? three, [DisallowNull] in string? four)
        {
            Console.WriteLine(one.Length);
            Console.WriteLine(two.Length);
            Console.WriteLine(three.Length);
            Console.WriteLine(four.Length);
        }

        [AllowNull]
        string One
        { 
            set => Console.WriteLine(value.Length);
        }

        [DisallowNull]
        string? Two
        { 
            set => Console.WriteLine(value.Length);
        }

        [AllowNull]
        string this[int x]
        {
            set => Console.WriteLine(value.Length);
        }
        
        [DisallowNull]
        string? this[bool x]
        {
            set => Console.WriteLine(value.Length);
        }

        void Test()
        {
            Method(null, null, null, null);

            One = null;
            Two = null;

            this[1] = null;
            this[true] = null;
        }
    }

    class PostConditions
    {
        void Method([MaybeNull] out string one, [NotNull] out string? two)
        {
            one = null;
            two = null;
        }

        [return: MaybeNull]
        string Method2() => null;

        [return: NotNull]
        string? Method3() => null;

        [MaybeNull]
        string One => null;

        [NotNull]
        string? Two => null;

        [MaybeNull]
        string this[int x] => null;

        [NotNull]
        string? this[bool x] => null;

        void Test()
        {
            Method(out var one, out var two);
            Console.WriteLine(one.Length);
            Console.WriteLine(two.Length);

            Console.WriteLine(Method2().Length);
            Console.WriteLine(Method3().Length);

            Console.WriteLine(One.Length);
            Console.WriteLine(Two.Length);

            Console.WriteLine(this[1].Length);
            Console.WriteLine(this[true].Length);
        }

        void Test(IEnumerable<string> sequence)
        {
            var item = sequence.FirstOrDefault();
            Console.WriteLine(item.Length);
        }
    }
}