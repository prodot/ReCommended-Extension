using System;
using System.Collections.Generic;

namespace TargetReadOnlyList
{
    public class NonGenericClass
    {
        IReadOnlyList<int> field01 = new List<int>();
        IReadOnlyList<int> field02 = new List<int>() { 1, 2, 3 };
        IReadOnlyList<int> field03 = new List<int>(8);
        IReadOnlyList<int> field04 = new List<int>(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            IReadOnlyList<int> var01 = new List<int>();
            IReadOnlyList<int> var02 = new List<int> { a, b, c };
            IReadOnlyList<int> var03 = new List<int>(8);
            IReadOnlyList<int> var04 = new List<int>(8) { a, b, c };
            IReadOnlyList<int> var05 = new List<int>(seq);
            IReadOnlyList<int> var06 = new List<int>(seq) { a, b, c };

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

        void Consumer(IReadOnlyList<int> items) { }
        void ConsumerGeneric<T>(IReadOnlyList<T> items) { }

        IReadOnlyList<int> Property01 { get; } = new List<int>();
        IReadOnlyList<int> Property02 { get; } = new List<int> { 1, 2, 3 };
        IReadOnlyList<int> Property03 { get; set; } = new List<int>(8);
        IReadOnlyList<int> Property04 { get; set; } = new List<int>(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        IReadOnlyList<T> field01 = new List<T>();
        IReadOnlyList<T> field02 = new List<T> { default, default(T), new() };
        IReadOnlyList<T> field03 = new List<T>(8);
        IReadOnlyList<T> field04 = new List<T>(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq)
        {
            IReadOnlyList<T> var01 = new List<T>();
            IReadOnlyList<T> var02 = new List<T> { a, b, c };
            IReadOnlyList<T> var03 = new List<T>(8);
            IReadOnlyList<T> var04 = new List<T>(8) { a, b, c };
            IReadOnlyList<T> var05 = new List<T>(seq);
            IReadOnlyList<T> var06 = new List<T>(seq) { a, b, c };

            Consumer(new List<T>());
            Consumer(new List<T> { a, b, c });
            Consumer(new List<T>(8));
            Consumer(new List<T>(8) { a, b, c });
            Consumer(new List<T>(seq));
            Consumer(new List<T>(seq) { a, b, c });
        }

        void Consumer(IReadOnlyList<T> items) { }

        IReadOnlyList<T> Property01 { get; } = new List<T>();
        IReadOnlyList<T> Property02 { get; } = new List<T> { default, default(T), new() };
        IReadOnlyList<T> Property03 { get; set; } = new List<T>(8);
        IReadOnlyList<T> Property04 { get; set; } = new List<T>(8) { default, default(T), new() };
    }
}