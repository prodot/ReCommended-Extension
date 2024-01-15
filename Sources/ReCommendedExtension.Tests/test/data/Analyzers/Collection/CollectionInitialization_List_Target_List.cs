using System;
using System.Collections.Generic;

namespace TargetList
{
    public class NonGenericClass
    {
        List<int> field01 = new List<int>();
        List<int> field02 = new List<int>() { 1, 2, 3 };
        List<int> field03 = new List<int>(8);
        List<int> field04 = new List<int>(8) { 1, 2, 3 };
        List<int> field07 = new();
        List<int> field08 = new() { 1, 2, 3 };
        List<int> field09 = new(8);
        List<int> field10 = new(8) { 1, 2, 3 };

        void Method(int a, int b, int c, IEnumerable<int> seq)
        {
            List<int> var01 = new List<int>();
            List<int> var02 = new List<int> { a, b, c };
            List<int> var03 = new List<int>(8);
            List<int> var04 = new List<int>(8) { a, b, c };
            List<int> var05 = new List<int>(seq);
            List<int> var06 = new List<int>(seq) { a, b, c };
            List<int> var07 = new();
            List<int> var08 = new() { a, b, c };
            List<int> var09 = new(8);
            List<int> var10 = new(8) { a, b, c};
            List<int> var11 = new(seq);
            List<int> var12 = new(seq) { a, b, c };

            Consumer(new List<int>());
            Consumer(new List<int> { a, b, c });
            Consumer(new List<int>(8));
            Consumer(new List<int>(8) { a, b, c });
            Consumer(new List<int>(seq));
            Consumer(new List<int>(seq) { a, b, c });
            Consumer(new());
            Consumer(new() { a, b, c });
            Consumer(new(8));
            Consumer(new(8) { a, b, c });
            Consumer(new(seq));
            Consumer(new(seq) { a, b, c });

            ConsumerGeneric(new List<int>());
            ConsumerGeneric(new List<int> { a, b, c });
            ConsumerGeneric<int>(new List<int>(8));
            ConsumerGeneric<int>(new List<int>(8) { a, b, c });
            ConsumerGeneric(new List<int>(seq));
            ConsumerGeneric(new List<int>(seq) { a, b, c });
        }

        void Consumer(List<int> items) { }
        void ConsumerGeneric<T>(List<T> items) { }

        List<int> Property01 { get; } = new List<int>();
        List<int> Property02 { get; } = new List<int> { 1, 2, 3 };
        List<int> Property03 { get; set; } = new List<int>(8);
        List<int> Property04 { get; set; } = new List<int>(8) { 1, 2, 3 };
        List<int> Property07 => new();
        List<int> Property08 => new() { 1, 2, 3 };
        List<int> Property09 => new(8);
        List<int> Property10 => new(8) { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        List<T> field01 = new List<T>();
        List<T> field02 = new List<T> { default, default(T), new() };
        List<T> field03 = new List<T>(8);
        List<T> field04 = new List<T>(8) { default, default(T), new() };
        List<T> field07 = new();
        List<T> field08 = new() { default, default(T), new() };
        List<T> field09 = new(8);
        List<T> field10 = new(8) { default, default(T), new() };

        void Method(T a, T b, T c, IEnumerable<T> seq)
        {
            List<T> var01 = new List<T>();
            List<T> var02 = new List<T> { a, b, c };
            List<T> var03 = new List<T>(8);
            List<T> var04 = new List<T>(8) { a, b, c };
            List<T> var05 = new List<T>(seq);
            List<T> var06 = new List<T>(seq) { a, b, c };
            List<T> var07 = new();
            List<T> var08 = new() { a, b, c };
            List<T> var09 = new(8);
            List<T> var10 = new(8) { a, b, c };
            List<T> var11 = new(seq);
            List<T> var12 = new(seq) { a, b, c };

            Consumer(new List<T>());
            Consumer(new List<T> { a, b, c });
            Consumer(new List<T>(8));
            Consumer(new List<T>(8) { a, b, c });
            Consumer(new List<T>(seq));
            Consumer(new List<T>(seq) { a, b, c });
            Consumer(new());
            Consumer(new() { a, b, c });
            Consumer(new(8));
            Consumer(new(8) { a, b, c });
            Consumer(new(seq));
            Consumer(new(seq) { a, b, c });
        }

        void Consumer(List<T> items) { }

        List<T> Property01 { get; } = new List<T>();
        List<T> Property02 { get; } = new List<T> { default, default(T), new() };
        List<T> Property03 { get; set; } = new List<T>(8);
        List<T> Property04 { get; set; } = new List<T>(8) { default, default(T), new() };
        List<T> Property07 => new();
        List<T> Property08 => new() { default, default(T), new() };
        List<T> Property09 => new(8);
        List<T> Property10 => new(8) { default, default(T), new() };
    }

    internal class A { }
    internal class B(int x = 0) : A { }

    public class InferenceClass
    {
        List<A> field08 = new() { new B(1), new B(2), new B(3) };
        List<A> field10 = new(8) { new B(1), new B(2), new B(3) };

        void Method(B a, B b, B c, IEnumerable<B> seq)
        {
            List<A> var08 = new() { a, b, c };
            List<A> var10 = new(8) { a, b, c };
            List<A> var11 = new(seq);
            List<A> var12 = new(seq) { a, b, c };

            Consumer(new() { a, b, c });
            Consumer(new(8) { a, b, c });
            Consumer(new(seq));
            Consumer(new(seq) { a, b, c });
        }

        void Consumer(List<A> items) { }

        List<A> Property08 => new() { new B(1), new B(2), new B(3) };
        List<A> Property10 => new(8) { new B(1), new B(2), new B(3) };
    }
}