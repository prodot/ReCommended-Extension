using System;
using System.Collections.Generic;

namespace TargetHashSet
{
    public class NonGenericClass
    {
        HashSet<int> field01 = new HashSet<int>();
        HashSet<int> field02 = new HashSet<int> { 1, 2, 3 };
        HashSet<int> field03 = new HashSet<int>(8);
        HashSet<int> field04 = new HashSet<int>(8) { 1, 2, 3 };
        HashSet<int> field13 = new();
        HashSet<int> field14 = new() { 1, 2, 3 };
        HashSet<int> field15 = new(8);
        HashSet<int> field16 = new(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq, IEqualityComparer<int> comparer)
        {
            HashSet<int> var01 = new HashSet<int>();
            HashSet<int> var02 = new HashSet<int> { a, b, c };
            HashSet<int> var03 = new HashSet<int>(8);
            HashSet<int> var04 = new HashSet<int>(8) { a, b, c };
            HashSet<int> var05 = new HashSet<int>(seq);
            HashSet<int> var06 = new HashSet<int>(seq) { a, b, c };
            HashSet<int> var07 = new HashSet<int>(comparer);
            HashSet<int> var08 = new HashSet<int>(comparer) { a, b, c };
            HashSet<int> var09 = new HashSet<int>(8, comparer);
            HashSet<int> var10 = new HashSet<int>(8, comparer) { a, b, c };
            HashSet<int> var11 = new HashSet<int>(seq, comparer);
            HashSet<int> var12 = new HashSet<int>(seq, comparer) { a, b, c };
            HashSet<int> var13 = new();
            HashSet<int> var14 = new() { a, b, c };
            HashSet<int> var15 = new(8);
            HashSet<int> var16 = new(8) { a, b, c };
            HashSet<int> var17 = new(seq);
            HashSet<int> var18 = new(seq) { a, b, c };
            HashSet<int> var19 = new(comparer);
            HashSet<int> var20 = new(comparer) { a, b, c };
            HashSet<int> var21 = new(8, comparer);
            HashSet<int> var22 = new(8, comparer) { a, b, c };
            HashSet<int> var23 = new(seq, comparer);
            HashSet<int> var24 = new(seq, comparer) { a, b, c };

            Consumer(new HashSet<int>());
            Consumer(new HashSet<int> { a, b, c });
            Consumer(new HashSet<int>(8));
            Consumer(new HashSet<int>(8) { a, b, c });
            Consumer(new HashSet<int>(seq));
            Consumer(new HashSet<int>(seq) { a, b, c });
            Consumer(new HashSet<int>(comparer));
            Consumer(new HashSet<int>(comparer) { a, b, c });
            Consumer(new HashSet<int>(8, comparer));
            Consumer(new HashSet<int>(8, comparer) { a, b, c });
            Consumer(new HashSet<int>(seq, comparer));
            Consumer(new HashSet<int>(seq, comparer) { a, b, c });
            Consumer(new());
            Consumer(new() { a, b, c });
            Consumer(new(8));
            Consumer(new(8) { a, b, c });
            Consumer(new(seq));
            Consumer(new(seq) { a, b, c });
            Consumer(new(comparer));
            Consumer(new(comparer) { a, b, c });
            Consumer(new(8, comparer));
            Consumer(new(8, comparer) { a, b, c });
            Consumer(new(seq, comparer));
            Consumer(new(seq, comparer) { a, b, c });

            ConsumerGeneric(new HashSet<int>());
            ConsumerGeneric(new HashSet<int> { a, b, c });
            ConsumerGeneric<int>(new HashSet<int>(8));
            ConsumerGeneric<int>(new HashSet<int>(8) { a, b, c });
            ConsumerGeneric(new HashSet<int>(seq));
            ConsumerGeneric(new HashSet<int>(seq) { a, b, c });
            ConsumerGeneric(new HashSet<int>(comparer));
            ConsumerGeneric(new HashSet<int>(comparer) { a, b, c });
            ConsumerGeneric(new HashSet<int>(8, comparer));
            ConsumerGeneric(new HashSet<int>(8, comparer) { a, b, c });
            ConsumerGeneric(new HashSet<int>(seq, comparer));
            ConsumerGeneric(new HashSet<int>(seq, comparer) { a, b, c });
        }

        void Consumer(HashSet<int> items) { }
        void ConsumerGeneric<T>(HashSet<T> items) { }

        HashSet<int> Property01 { get; } = new HashSet<int>();
        HashSet<int> Property02 { get; } = new HashSet<int> { 1, 2, 3 };
        HashSet<int> Property03 { get; set; } = new HashSet<int>(8);
        HashSet<int> Property04 { get; set; } = new HashSet<int>(8) { 1, 2, 3 };
        HashSet<int> Property13 => new();
        HashSet<int> Property14 => new() { 1, 2, 3 };
        HashSet<int> Property15 => new(8);
        HashSet<int> Property16 => new(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        HashSet<T> field01 = new HashSet<T>();
        HashSet<T> field02 = new HashSet<T> { default, default(T), new() };
        HashSet<T> field03 = new HashSet<T>(8);
        HashSet<T> field04 = new HashSet<T>(8) { default, default(T), new() };
        HashSet<T> field13 = new();
        HashSet<T> field14 = new() { default, default(T), new() };
        HashSet<T> field15 = new(8);
        HashSet<T> field16 = new(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq, IEqualityComparer<T> comparer)
        {
            HashSet<T> var01 = new HashSet<T>();
            HashSet<T> var02 = new HashSet<T> { a, b, c };
            HashSet<T> var03 = new HashSet<T>(8);
            HashSet<T> var04 = new HashSet<T>(8) { a, b, c };
            HashSet<T> var05 = new HashSet<T>(seq);
            HashSet<T> var06 = new HashSet<T>(seq) { a, b, c };
            HashSet<T> var07 = new HashSet<T>(comparer);
            HashSet<T> var08 = new HashSet<T>(comparer) { a, b, c };
            HashSet<T> var09 = new HashSet<T>(8, comparer);
            HashSet<T> var10 = new HashSet<T>(8, comparer) { a, b, c };
            HashSet<T> var11 = new HashSet<T>(seq, comparer);
            HashSet<T> var12 = new HashSet<T>(seq, comparer) { a, b, c };
            HashSet<T> var13 = new();
            HashSet<T> var14 = new() { a, b, c };
            HashSet<T> var15 = new(8);
            HashSet<T> var16 = new(8) { a, b, c };
            HashSet<T> var17 = new(seq);
            HashSet<T> var18 = new(seq) { a, b, c };
            HashSet<T> var19 = new(comparer);
            HashSet<T> var20 = new(comparer) { a, b, c };
            HashSet<T> var21 = new(8, comparer);
            HashSet<T> var22 = new(8, comparer) { a, b, c };
            HashSet<T> var23 = new(seq, comparer);
            HashSet<T> var24 = new(seq, comparer) { a, b, c };

            Consumer(new HashSet<T>());
            Consumer(new HashSet<T> { a, b, c });
            Consumer(new HashSet<T>(8));
            Consumer(new HashSet<T>(8) { a, b, c });
            Consumer(new HashSet<T>(seq));
            Consumer(new HashSet<T>(seq) { a, b, c });
            Consumer(new HashSet<T>(comparer));
            Consumer(new HashSet<T>(comparer) { a, b, c });
            Consumer(new HashSet<T>(8, comparer));
            Consumer(new HashSet<T>(8, comparer) { a, b, c });
            Consumer(new HashSet<T>(seq, comparer));
            Consumer(new HashSet<T>(seq, comparer) { a, b, c });
            Consumer(new());
            Consumer(new() { a, b, c });
            Consumer(new(8));
            Consumer(new(8) { a, b, c });
            Consumer(new(seq));
            Consumer(new(seq) { a, b, c });
            Consumer(new(comparer));
            Consumer(new(comparer) { a, b, c });
            Consumer(new(8, comparer));
            Consumer(new(8, comparer) { a, b, c });
            Consumer(new(seq, comparer));
            Consumer(new(seq, comparer) { a, b, c });
        }

        void Consumer(HashSet<T> items) { }

        HashSet<T> Property01 { get; } = new HashSet<T>();
        HashSet<T> Property02 { get; } = new HashSet<T> { 1, 2, 3 };
        HashSet<T> Property03 { get; set; } = new HashSet<T>(8);
        HashSet<T> Property04 { get; set; } = new HashSet<T>(8) { 1, 2, 3 };
        HashSet<T> Property13 => new();
        HashSet<T> Property14 => new() { 1, 2, 3 };
        HashSet<T> Property15 => new(8);
        HashSet<T> Property16 => new(8) { 1, 2, 3 };
    }
}