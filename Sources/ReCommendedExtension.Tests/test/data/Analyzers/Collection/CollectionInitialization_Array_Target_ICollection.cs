using System;
using System.Collections.Generic;

namespace TargetCollection
{
    public class NonGenericClass
    {
        ICollection<int> field1 = new int[] { };
        ICollection<int> field2 = new int[] { 1, 2, 3 };
        ICollection<int> field3 = new int[0] { };
        ICollection<int> field4 = new int[3] { 1, 2, 3 };
        ICollection<int> field5 = new int[0];
        ICollection<int> field6 = new int[3];
        ICollection<int> field7 = new[] { 1, 2, 3 };
        ICollection<int> field8 = Array.Empty<int>();

        void Method(int a, int b, int c)
        {
            ICollection<int> var1 = new int[] { };
            ICollection<int> var2 = new int[] { a, b, c };
            ICollection<int> var3 = new int[0] { };
            ICollection<int> var4 = new int[3] { a, b, c };
            ICollection<int> var5 = new int[0];
            ICollection<int> var6 = new int[3];
            ICollection<int> var7 = new[] { a, b, c };
            ICollection<int> var8 = Array.Empty<int>();

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

        void Consumer(ICollection<int> items) { }
        void ConsumerGeneric<T>(ICollection<T> items) { }

        ICollection<string?> Property1 { get; } = new string?[] { };
        ICollection<string?> Property2 { get; } = new string?[] { "one", "two", "three" };
        ICollection<string?> Property3 { get; set; } = new string?[0] { };
        ICollection<string?> Property4 { get; set; } = new string?[3] { "one", "two", "three" };
        ICollection<string?> Property5 => new string?[0];
        ICollection<string?> Property6 => new string?[3];
        ICollection<string?> Property7 => new[] { "one", "two", "three", null };
        ICollection<string?> Property8 => Array.Empty<string>();
    }

    public class GenericClass<T> where T : new()
    {
        ICollection<T> field1 = new T[] { };
        ICollection<T> field2 = new T[] { default, default(T), new() };
        ICollection<T> field3 = new T[0] { };
        ICollection<T> field4 = new T[3] { default, default(T), new() };
        ICollection<T> field5 = new T[0];
        ICollection<T> field6 = new T[3];
        ICollection<T> field7 = new[] { default, default(T), new() };
        ICollection<T> field8 = Array.Empty<T>();

        void Method(T a, T b, T c)
        {
            ICollection<T> var1 = new T[] { };
            ICollection<T> var2 = new T[] { a, b, c };
            ICollection<T> var3 = new T[0] { };
            ICollection<T> var4 = new T[3] { a, b, c };
            ICollection<T> var5 = new T[0];
            ICollection<T> var6 = new T[3];
            ICollection<T> var7 = new[] { a, b, c };
            ICollection<T> var8 = Array.Empty<T>();

            Consumer(new T[] { });
            Consumer(new T[] { a, b, c });
            Consumer(new T[0] { });
            Consumer(new T[3] { a, b, c });
            Consumer(new T[0]);
            Consumer(new T[3]);
            Consumer(new[] { a, b, c });
            Consumer(Array.Empty<T>());
        }

        void Consumer(ICollection<T> items) { }

        ICollection<T> Property1 { get; } = new T[] { };
        ICollection<T> Property2 { get; } = new T[] { default, default(T), new() };
        ICollection<T> Property3 { get; set; } = new T[0] { };
        ICollection<T> Property4 { get; set; } = new T[3] { default, default(T), new() };
        ICollection<T> Property5 => new T[0];
        ICollection<T> Property6 => new T[3];
        ICollection<T> Property7 => new[] { default, default(T), new() };
        ICollection<T> Property8 => Array.Empty<T>();
    }

    internal class A { }
    internal class B(int x = 0) : A { }

    public class InferenceClass
    {
        ICollection<A> field1 = new B[] { };
        ICollection<A> field2 = new B[] { new(1), new(2), new(3) };
        ICollection<A> field3 = new B[0] { };
        ICollection<A> field4 = new B[3] { new B(1), new B(2), new B(3) };
        ICollection<A> field5 = new B[0];
        ICollection<A> field6 = new B[3];
        ICollection<A> field7 = new[] { new B(1), new B(2), new B(3) };
        ICollection<A> field8 = Array.Empty<B>();

        void Method(B a, B b, B c)
        {
            ICollection<A> var1 = new B[] { };
            ICollection<A> var2 = new B[] { a, b, c };
            ICollection<A> var3 = new B[0] { };
            ICollection<A> var4 = new B[3] { a, b, c };
            ICollection<A> var5 = new B[0];
            ICollection<A> var6 = new B[3];
            ICollection<A> var7 = new[] { a, b, c };
            ICollection<A> var8 = Array.Empty<B>();

            Consumer(new B[] { });
            Consumer(new B[] { a, b, c });
            Consumer(new B[0] { });
            Consumer(new B[3] { a, b, c });
            Consumer(new B[0]);
            Consumer(new B[3]);
            Consumer(new[] { a, b, c });
            Consumer(Array.Empty<B>());
        }

        void Consumer(ICollection<A> items) { }

        ICollection<A> Property1 { get; } = new B[] { };
        ICollection<A> Property2 { get; } = new B[] { new(1), new(2), new(3) };
        ICollection<A> Property3 { get; set; } = new B[0] { };
        ICollection<A> Property4 { get; set; } = new B[3] { new B(1), new B(2), new B(3) };
        ICollection<A> Property5 => new B[0];
        ICollection<A> Property6 => new B[3];
        ICollection<A> Property7 => new[] { new B(1), new B(2), new B(3) };
        ICollection<A> Property8 => Array.Empty<B>();
    }
}