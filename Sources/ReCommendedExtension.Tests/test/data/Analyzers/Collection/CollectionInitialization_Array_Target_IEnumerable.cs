using System;
using System.Collections.Generic;

namespace TargetEnumerable
{
    public class NonGenericClass
    {
        IEnumerable<int> field1 = new int[] { };
        IEnumerable<int> field2 = new int[] { 1, 2, 3 };
        IEnumerable<int> field3 = new int[0] { };
        IEnumerable<int> field4 = new int[3] { 1, 2, 3 };
        IEnumerable<int> field5 = new int[0];
        IEnumerable<int> field6 = new int[3];
        IEnumerable<int> field7 = new[] { 1, 2, 3 };
        IEnumerable<int> field8 = Array.Empty<int>();

        void Method(int a, int b, int c)
        {
            IEnumerable<int> var1 = new int[] { };
            IEnumerable<int> var2 = new int[] { a, b, c };
            IEnumerable<int> var3 = new int[0] { };
            IEnumerable<int> var4 = new int[3] { a, b, c };
            IEnumerable<int> var5 = new int[0];
            IEnumerable<int> var6 = new int[3];
            IEnumerable<int> var7 = new[] { a, b, c };
            IEnumerable<int> var8 = Array.Empty<int>();

            Consumer(new int[] { });
            Consumer(new int[] { a, b, c });
            Consumer(new int[0] { });
            Consumer(new int[3] { a, b, c });
            Consumer(new int[0]);
            Consumer(new int[3]);
            Consumer(new[] { a, b, c });
            Consumer(Array.Empty<int>());

            ConsumerGeneric(new int[] { });
            ConsumerGeneric(new int[] { a, b, c });
            ConsumerGeneric<int>(new int[0] { });
            ConsumerGeneric(new int[3] { a, b, c });
            ConsumerGeneric(new int[0]);
            ConsumerGeneric(new int[3]);
            ConsumerGeneric(new[] { a, b, c });
            ConsumerGeneric(Array.Empty<int>());
        }

        void Consumer(IEnumerable<int> items) { }
        void ConsumerGeneric<T>(IEnumerable<T> items) { }

        IEnumerable<int> Property1 { get; } = new int[] { };
        IEnumerable<int> Property2 { get; } = new int[] { 1, 2, 3 };
        IEnumerable<int> Property3 { get; set; } = new int[0] { };
        IEnumerable<int> Property4 { get; set; } = new int[3] { 1, 2, 3 };
        IEnumerable<int> Property5 => new int[0];
        IEnumerable<int> Property6 => new int[3];
        IEnumerable<int> Property7 => new[] { 1, 2, 3 };
        IEnumerable<int> Property8 => Array.Empty<int>();
    }

    public class GenericClass<T> where T : new()
    {
        IEnumerable<T> field1 = new T[] { };
        IEnumerable<T> field2 = new T[] { default, default(T), new() };
        IEnumerable<T> field3 = new T[0] { };
        IEnumerable<T> field4 = new T[3] { default, default(T), new() };
        IEnumerable<T> field5 = new T[0];
        IEnumerable<T> field6 = new T[3];
        IEnumerable<T> field7 = new[] { default, default(T), new() };
        IEnumerable<T> field8 = Array.Empty<T>();

        void Method(T a, T b, T c)
        {
            IEnumerable<T> var1 = new T[] { };
            IEnumerable<T> var2 = new T[] { a, b, c };
            IEnumerable<T> var3 = new T[0] { };
            IEnumerable<T> var4 = new T[3] { a, b, c };
            IEnumerable<T> var5 = new T[0];
            IEnumerable<T> var6 = new T[3];
            IEnumerable<T> var7 = new[] { a, b, c };
            IEnumerable<T> var8 = Array.Empty<T>();

            Consumer(new T[] { });
            Consumer(new T[] { a, b, c });
            Consumer(new T[0] { });
            Consumer(new T[3] { a, b, c });
            Consumer(new T[0]);
            Consumer(new T[3]);
            Consumer(new[] { a, b, c });
            Consumer(Array.Empty<T>());
        }

        void Consumer(IEnumerable<T> items) { }

        IEnumerable<T> Property1 { get; } = new T[] { };
        IEnumerable<T> Property2 { get; } = new T[] { default, default(T), new() };
        IEnumerable<T> Property3 { get; set; } = new T[0] { };
        IEnumerable<T> Property4 { get; set; } = new T[3] { default, default(T), new() };
        IEnumerable<T> Property5 => new T[0];
        IEnumerable<T> Property6 => new T[3];
        IEnumerable<T> Property7 => new[] { default, default(T), new() };
        IEnumerable<T> Property8 => Array.Empty<T>();
    }

    internal class A { }
    internal class B(int x = 0) : A { }

    public class InferenceClass
    {
        IEnumerable<A> field1 = new B[] { };
        IEnumerable<A> field2 = new B[] { new(1), new(2), new(3) };
        IEnumerable<A> field3 = new B[0] { };
        IEnumerable<A> field4 = new B[3] { new B(1), new B(2), new B(3) };
        IEnumerable<A> field5 = new B[0];
        IEnumerable<A> field6 = new B[3];
        IEnumerable<A> field7 = new[] { new B(1), new B(2), new B(3) };
        IEnumerable<A> field8 = Array.Empty<B>();

        void Method(B a, B b, B c)
        {
            IEnumerable<A> var1 = new B[] { };
            IEnumerable<A> var2 = new B[] { a, b, c };
            IEnumerable<A> var3 = new B[0] { };
            IEnumerable<A> var4 = new B[3] { a, b, c };
            IEnumerable<A> var5 = new B[0];
            IEnumerable<A> var6 = new B[3];
            IEnumerable<A> var7 = new[] { a, b, c };
            IEnumerable<A> var8 = Array.Empty<B>();

            Consumer(new B[] { });
            Consumer(new B[] { a, b, c });
            Consumer(new B[0] { });
            Consumer(new B[3] { a, b, c });
            Consumer(new B[0]);
            Consumer(new B[3]);
            Consumer(new[] { a, b, c });
            Consumer(Array.Empty<B>());
        }

        void Consumer(IEnumerable<A> items) { }

        IEnumerable<A> Property1 { get; } = new B[] { };
        IEnumerable<A> Property2 { get; } = new B[] { new(1), new(2), new(3) };
        IEnumerable<A> Property3 { get; set; } = new B[0] { };
        IEnumerable<A> Property4 { get; set; } = new B[3] { new B(1), new B(2), new B(3) };
        IEnumerable<A> Property5 => new B[0];
        IEnumerable<A> Property6 => new B[3];
        IEnumerable<A> Property7 => new[] { new B(1), new B(2), new B(3) };
        IEnumerable<A> Property8 => Array.Empty<B>();
    }
}