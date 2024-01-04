using System;
using System.Collections.Generic;

namespace TargetCollection
{
    public class NonGenericClass
    {
        ICollection<int> field01 = new List<int>();
        ICollection<int> field02 = new List<int>() { 1, 2, 3 };
        ICollection<int> field03 = new List<int>(8);
        ICollection<int> field04 = new List<int>(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            ICollection<int> var01 = new List<int>();
            ICollection<int> var02 = new List<int> { a, b, c };
            ICollection<int> var03 = new List<int>(8);
            ICollection<int> var04 = new List<int>(8) { a, b, c };
            ICollection<int> var05 = new List<int>(seq);
            ICollection<int> var06 = new List<int>(seq) { a, b, c };

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

        void Consumer(ICollection<int> items) { }
        void ConsumerGeneric<T>(ICollection<T> items) { }

        ICollection<int> Property01 { get; } = new List<int>();
        ICollection<int> Property02 { get; } = new List<int> { 1, 2, 3 };
        ICollection<int> Property03 { get; set; } = new List<int>(8);
        ICollection<int> Property04 { get; set; } = new List<int>(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        ICollection<T> field01 = new List<T>();
        ICollection<T> field02 = new List<T> { default, default(T), new() };
        ICollection<T> field03 = new List<T>(8);
        ICollection<T> field04 = new List<T>(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq)
        {
            ICollection<T> var01 = new List<T>();
            ICollection<T> var02 = new List<T> { a, b, c };
            ICollection<T> var03 = new List<T>(8);
            ICollection<T> var04 = new List<T>(8) { a, b, c };
            ICollection<T> var05 = new List<T>(seq);
            ICollection<T> var06 = new List<T>(seq) { a, b, c };

            Consumer(new List<T>());
            Consumer(new List<T> { a, b, c });
            Consumer(new List<T>(8));
            Consumer(new List<T>(8) { a, b, c });
            Consumer(new List<T>(seq));
            Consumer(new List<T>(seq) { a, b, c });
        }

        void Consumer(ICollection<T> items) { }

        ICollection<T> Property01 { get; } = new List<T>();
        ICollection<T> Property02 { get; } = new List<T> { default, default(T), new() };
        ICollection<T> Property03 { get; set; } = new List<T>(8);
        ICollection<T> Property04 { get; set; } = new List<T>(8) { default, default(T), new() };
    }
}