using System;
using System.Collections.Generic;

namespace TargetReadOnlyCollection
{
    public class NonGenericClass
    {
        IReadOnlyCollection<int> field01 = new List<int>();
        IReadOnlyCollection<int> field02 = new List<int>() { 1, 2, 3 };
        IReadOnlyCollection<int> field03 = new List<int>(8);
        IReadOnlyCollection<int> field04 = new List<int>(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            IReadOnlyCollection<int> var01 = new List<int>();
            IReadOnlyCollection<int> var02 = new List<int> { a, b, c };
            IReadOnlyCollection<int> var03 = new List<int>(8);
            IReadOnlyCollection<int> var04 = new List<int>(8) { a, b, c };
            IReadOnlyCollection<int> var05 = new List<int>(seq);
            IReadOnlyCollection<int> var06 = new List<int>(seq) { a, b, c };

            Consumer(new List<int>());
            Consumer(new List<int> { a, b, c });
            Consumer(new List<int>(8));
            Consumer(new List<int>(8) { a, b, c });
            Consumer(new List<int>(seq));
            Consumer(new List<int>(seq) { a, b, c });

            ConsumerGeneric(new List<int>());
            ConsumerGeneric(new List<int> { a, b, c });
            ConsumerGeneric<int>(new List<int>(8));
            ConsumerGeneric<int>(new List<int>(8) { a, b, c });
            ConsumerGeneric(new List<int>(seq));
            ConsumerGeneric(new List<int>(seq) { a, b, c });
        }

        void Consumer(IReadOnlyCollection<int> items) { }
        void ConsumerGeneric<T>(IReadOnlyCollection<T> items) { }

        IReadOnlyCollection<int> Property01 { get; } = new List<int>();
        IReadOnlyCollection<int> Property02 { get; } = new List<int> { 1, 2, 3 };
        IReadOnlyCollection<int> Property03 { get; set; } = new List<int>(8);
        IReadOnlyCollection<int> Property04 { get; set; } = new List<int>(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        IReadOnlyCollection<T> field01 = new List<T>();
        IReadOnlyCollection<T> field02 = new List<T> { default, default(T), new() };
        IReadOnlyCollection<T> field03 = new List<T>(8);
        IReadOnlyCollection<T> field04 = new List<T>(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq)
        {
            IReadOnlyCollection<T> var01 = new List<T>();
            IReadOnlyCollection<T> var02 = new List<T> { a, b, c };
            IReadOnlyCollection<T> var03 = new List<T>(8);
            IReadOnlyCollection<T> var04 = new List<T>(8) { a, b, c };
            IReadOnlyCollection<T> var05 = new List<T>(seq);
            IReadOnlyCollection<T> var06 = new List<T>(seq) { a, b, c };

            Consumer(new List<T>());
            Consumer(new List<T> { a, b, c });
            Consumer(new List<T>(8));
            Consumer(new List<T>(8) { a, b, c });
            Consumer(new List<T>(seq));
            Consumer(new List<T>(seq) { a, b, c });
        }

        void Consumer(IReadOnlyCollection<T> items) { }

        IReadOnlyCollection<T> Property01 { get; } = new List<T>();
        IReadOnlyCollection<T> Property02 { get; } = new List<T> { default, default(T), new() };
        IReadOnlyCollection<T> Property03 { get; set; } = new List<T>(8);
        IReadOnlyCollection<T> Property04 { get; set; } = new List<T>(8) { default, default(T), new() };
    }
}