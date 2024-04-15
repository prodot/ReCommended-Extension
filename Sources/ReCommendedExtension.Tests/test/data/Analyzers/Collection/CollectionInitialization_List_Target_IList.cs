using System;
using System.Collections.Generic;

namespace TargetList
{
    public class NonGenericClass
    {
        IList<int> field01 = new List<int>();
        IList<int> field02 = new List<int>() { 1, 2, 3 };
        IList<int> field03 = new List<int>(8);
        IList<int> field04 = new List<int>(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            IList<int> var01 = new List<int>();
            IList<int> var02 = new List<int> { a, b, c };
            IList<int> var03 = new List<int>(8);
            IList<int> var04 = new List<int>(8) { a, b, c };
            IList<int> var05 = new List<int>(seq);
            IList<int> var06 = new List<int>(seq) { a, b, c };

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

        void Consumer(IList<int> items) { }
        void ConsumerGeneric<T>(IList<T> items) { }

        IList<int> Property01 { get; } = new List<int>();
        IList<int> Property02 { get; } = new List<int> { 1, 2, 3 };
        IList<int> Property03 { get; set; } = new List<int>(8);
        IList<int> Property04 { get; set; } = new List<int>(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        IList<T> field01 = new List<T>();
        IList<T> field02 = new List<T> { default, default(T), new() };
        IList<T> field03 = new List<T>(8);
        IList<T> field04 = new List<T>(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq)
        {
            IList<T> var01 = new List<T>();
            IList<T> var02 = new List<T> { a, b, c };
            IList<T> var03 = new List<T>(8);
            IList<T> var04 = new List<T>(8) { a, b, c };
            IList<T> var05 = new List<T>(seq);
            IList<T> var06 = new List<T>(seq) { a, b, c };

            Consumer(new List<T>());
            Consumer(new List<T> { a, b, c });
            Consumer(new List<T>(8));
            Consumer(new List<T>(8) { a, b, c });
            Consumer(new List<T>(seq));
            Consumer(new List<T>(seq) { a, b, c });
        }

        void Consumer(IList<T> items) { }

        IList<T> Property01 { get; } = new List<T>();
        IList<T> Property02 { get; } = new List<T> { default, default(T), new() };
        IList<T> Property03 { get; set; } = new List<T>(8);
        IList<T> Property04 { get; set; } = new List<T>(8) { default, default(T), new() };
    }
}