using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        IEnumerable<int> field01 = new HashSet<int>();
        IEnumerable<int> field02 = new HashSet<int> { 1, 2, 3 };
        IEnumerable<int> field03 = new HashSet<int>(8);
        IEnumerable<int> field04 = new HashSet<int>(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq, IEqualityComparer<int> comparer)
        {
            IEnumerable<int> var01 = new HashSet<int>();
            IEnumerable<int> var02 = new HashSet<int> { a, b, c };
            IEnumerable<int> var03 = new HashSet<int>(8);
            IEnumerable<int> var04 = new HashSet<int>(8) { a, b, c };
            IEnumerable<int> var05 = new HashSet<int>(seq);
            IEnumerable<int> var06 = new HashSet<int>(seq) { a, b, c };
            IEnumerable<int> var07 = new HashSet<int>(comparer);
            IEnumerable<int> var08 = new HashSet<int>(comparer) { a, b, c };
            IEnumerable<int> var09 = new HashSet<int>(8, comparer);
            IEnumerable<int> var10 = new HashSet<int>(8, comparer) { a, b, c };
            IEnumerable<int> var11 = new HashSet<int>(seq, comparer);
            IEnumerable<int> var12 = new HashSet<int>(seq, comparer) { a, b, c };

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

        void Consumer(IEnumerable<int> items) { }
        void ConsumerGeneric<T>(IEnumerable<T> items) { }

        IEnumerable<int> Property01 { get; } = new HashSet<int>();
        IEnumerable<int> Property02 { get; } = new HashSet<int> { 1, 2, 3 };
        IEnumerable<int> Property03 { get; set; } = new HashSet<int>(8);
        IEnumerable<int> Property04 { get; set; } = new HashSet<int>(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        IEnumerable<T> field01 = new HashSet<T>();
        IEnumerable<T> field02 = new HashSet<T> { default, default(T), new() };
        IEnumerable<T> field03 = new HashSet<T>(8);
        IEnumerable<T> field04 = new HashSet<T>(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq, IEqualityComparer<T> comparer)
        {
            IEnumerable<T> var01 = new HashSet<T>();
            IEnumerable<T> var02 = new HashSet<T> { a, b, c };
            IEnumerable<T> var03 = new HashSet<T>(8);
            IEnumerable<T> var04 = new HashSet<T>(8) { a, b, c };
            IEnumerable<T> var05 = new HashSet<T>(seq);
            IEnumerable<T> var06 = new HashSet<T>(seq) { a, b, c };
            IEnumerable<T> var07 = new HashSet<T>(comparer);
            IEnumerable<T> var08 = new HashSet<T>(comparer) { a, b, c };
            IEnumerable<T> var09 = new HashSet<T>(8, comparer);
            IEnumerable<T> var10 = new HashSet<T>(8, comparer) { a, b, c };
            IEnumerable<T> var11 = new HashSet<T>(seq, comparer);
            IEnumerable<T> var12 = new HashSet<T>(seq, comparer) { a, b, c };

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
        }

        void Consumer(IEnumerable<T> items) { }

        IEnumerable<T> Property01 { get; } = new HashSet<T>();
        IEnumerable<T> Property02 { get; } = new HashSet<T> { 1, 2, 3 };
        IEnumerable<T> Property03 { get; set; } = new HashSet<T>(8);
        IEnumerable<T> Property04 { get; set; } = new HashSet<T>(8) { 1, 2, 3 };
    }
}