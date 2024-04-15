using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        IEnumerable<int> field01 = new List<int>();
        IEnumerable<int> field02 = new List<int>() { 1, 2, 3 };
        IEnumerable<int> field03 = new List<int>(8);
        IEnumerable<int> field04 = new List<int>(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            IEnumerable<int> var01 = new List<int>();
            IEnumerable<int> var02 = new List<int> { a, b, c };
            IEnumerable<int> var03 = new List<int>(8);
            IEnumerable<int> var04 = new List<int>(8) { a, b, c };
            IEnumerable<int> var05 = new List<int>(seq);
            IEnumerable<int> var06 = new List<int>(seq) { a, b, c };

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

        void Consumer(IEnumerable<int> items) { }
        void ConsumerGeneric<T>(IEnumerable<T> items) { }

        IEnumerable<int> Property01 { get; } = new List<int>();
        IEnumerable<int> Property02 { get; } = new List<int> { 1, 2, 3 };
        IEnumerable<int> Property03 { get; set; } = new List<int>(8);
        IEnumerable<int> Property04 { get; set; } = new List<int>(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        IEnumerable<T> field01 = new List<T>();
        IEnumerable<T> field02 = new List<T> { default, default(T), new() };
        IEnumerable<T> field03 = new List<T>(8);
        IEnumerable<T> field04 = new List<T>(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq)
        {
            IEnumerable<T> var01 = new List<T>();
            IEnumerable<T> var02 = new List<T> { a, b, c };
            IEnumerable<T> var03 = new List<T>(8);
            IEnumerable<T> var04 = new List<T>(8) { a, b, c };
            IEnumerable<T> var05 = new List<T>(seq);
            IEnumerable<T> var06 = new List<T>(seq) { a, b, c };

            Consumer(new List<T>());
            Consumer(new List<T> { a, b, c });
            Consumer(new List<T>(8));
            Consumer(new List<T>(8) { a, b, c });
            Consumer(new List<T>(seq));
            Consumer(new List<T>(seq) { a, b, c });
        }

        void Consumer(IEnumerable<T> items) { }

        IEnumerable<T> Property01 { get; } = new List<T>();
        IEnumerable<T> Property02 { get; } = new List<T> { default, default(T), new() };
        IEnumerable<T> Property03 { get; set; } = new List<T>(8);
        IEnumerable<T> Property04 { get; set; } = new List<T>(8) { default, default(T), new() };
    }

    internal class A { }
    internal class B(int x = 0) : A { }

    public class InferenceClass
    {
        IEnumerable<A> field01 = new List<B>();
        IEnumerable<A> field02 = new List<B>() { new(1), new(2), new(3) };
        IEnumerable<A> field03 = new List<B>(8);
        IEnumerable<A> field04 = new List<B>(8) { new B(1), new B(2), new B(3) };

        void Method(B a, B b, B c, IEnumerable<B> seq)
        {
            IEnumerable<A> var01 = new List<B>();
            IEnumerable<A> var02 = new List<B> { a, b, c };
            IEnumerable<A> var03 = new List<B>(8);
            IEnumerable<A> var04 = new List<B>(8) { a, b, c };
            IEnumerable<A> var05 = new List<B>(seq);
            IEnumerable<A> var06 = new List<B>(seq) { a, b, c };

            Consumer(new List<B>());
            Consumer(new List<B> { a, b, c });
            Consumer(new List<B>(8));
            Consumer(new List<B>(8) { a, b, c });
            Consumer(new List<B>(seq));
            Consumer(new List<B>(seq) { a, b, c });
        }

        void Consumer(IEnumerable<A> items) { }

        IEnumerable<A> Property01 { get; } = new List<B>();
        IEnumerable<A> Property02 { get; } = new List<B> { new(1), new(2), new(3) };
        IEnumerable<A> Property03 { get; set; } = new List<B>(8);
        IEnumerable<A> Property04 { get; set; } = new List<B>(8) { new B(1), new B(2), new B(3) };
    }
}