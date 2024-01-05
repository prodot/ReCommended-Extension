using System;
using System.Collections.Generic;

namespace TargetReadOnlyCollection
{
    public class NonGenericClass
    {
        IReadOnlyCollection<int> field01 = new HashSet<int>();
        IReadOnlyCollection<int> field02 = new HashSet<int> { 1, 2, 3 };
        IReadOnlyCollection<int> field03 = new HashSet<int>(8);
        IReadOnlyCollection<int> field04 = new HashSet<int>(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq, IEqualityComparer<int> comparer)
        {
            IReadOnlyCollection<int> var01 = new HashSet<int>();
            IReadOnlyCollection<int> var02 = new HashSet<int> { a, b, c };
            IReadOnlyCollection<int> var03 = new HashSet<int>(8);
            IReadOnlyCollection<int> var04 = new HashSet<int>(8) { a, b, c };
            IReadOnlyCollection<int> var05 = new HashSet<int>(seq);
            IReadOnlyCollection<int> var06 = new HashSet<int>(seq) { a, b, c };
            IReadOnlyCollection<int> var07 = new HashSet<int>(comparer);
            IReadOnlyCollection<int> var08 = new HashSet<int>(comparer) { a, b, c };
            IReadOnlyCollection<int> var09 = new HashSet<int>(8, comparer);
            IReadOnlyCollection<int> var10 = new HashSet<int>(8, comparer) { a, b, c };
            IReadOnlyCollection<int> var11 = new HashSet<int>(seq, comparer);
            IReadOnlyCollection<int> var12 = new HashSet<int>(seq, comparer) { a, b, c };

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

        void Consumer(IReadOnlyCollection<int> items) { }
        void ConsumerGeneric<T>(IReadOnlyCollection<T> items) { }

        IReadOnlyCollection<int> Property01 { get; } = new HashSet<int>();
        IReadOnlyCollection<int> Property02 { get; } = new HashSet<int> { 1, 2, 3 };
        IReadOnlyCollection<int> Property03 { get; set; } = new HashSet<int>(8);
        IReadOnlyCollection<int> Property04 { get; set; } = new HashSet<int>(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        IReadOnlyCollection<T> field01 = new HashSet<T>();
        IReadOnlyCollection<T> field02 = new HashSet<T> { default, default(T), new() };
        IReadOnlyCollection<T> field03 = new HashSet<T>(8);
        IReadOnlyCollection<T> field04 = new HashSet<T>(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq, IEqualityComparer<T> comparer)
        {
            IReadOnlyCollection<T> var01 = new HashSet<T>();
            IReadOnlyCollection<T> var02 = new HashSet<T> { a, b, c };
            IReadOnlyCollection<T> var03 = new HashSet<T>(8);
            IReadOnlyCollection<T> var04 = new HashSet<T>(8) { a, b, c };
            IReadOnlyCollection<T> var05 = new HashSet<T>(seq);
            IReadOnlyCollection<T> var06 = new HashSet<T>(seq) { a, b, c };
            IReadOnlyCollection<T> var07 = new HashSet<T>(comparer);
            IReadOnlyCollection<T> var08 = new HashSet<T>(comparer) { a, b, c };
            IReadOnlyCollection<T> var09 = new HashSet<T>(8, comparer);
            IReadOnlyCollection<T> var10 = new HashSet<T>(8, comparer) { a, b, c };
            IReadOnlyCollection<T> var11 = new HashSet<T>(seq, comparer);
            IReadOnlyCollection<T> var12 = new HashSet<T>(seq, comparer) { a, b, c };

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

        void Consumer(IReadOnlyCollection<T> items) { }

        IReadOnlyCollection<T> Property01 { get; } = new HashSet<T>();
        IReadOnlyCollection<T> Property02 { get; } = new HashSet<T> { 1, 2, 3 };
        IReadOnlyCollection<T> Property03 { get; set; } = new HashSet<T>(8);
        IReadOnlyCollection<T> Property04 { get; set; } = new HashSet<T>(8) { 1, 2, 3 };
    }
}