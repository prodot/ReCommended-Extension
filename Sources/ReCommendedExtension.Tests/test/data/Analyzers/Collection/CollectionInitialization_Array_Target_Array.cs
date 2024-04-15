using System;

namespace TargetArray
{
    public class NonGenericClass
    {
        int[] field01 = new int[] { };
        int[] field02 = new int[] { 1, 2, 3 };
        int[] field03 = new int[0] { };
        int[] field04 = new int[3] { 1, 2, 3 };
        int[] field05 = new int[0];
        int[] field06 = new int[3];
        int[] field07 = new[] { 1, 2, 3 };
        int[] field08 = { };
        int[] field09 = { 1, 2, 3 };
        int[] field10 = Array.Empty<int>();

        void Method(int a, int b, int c)
        {
            int[] var01 = new int[] { };
            int[] var02 = new int[] { a, b, c };
            int[] var03 = new int[0] { };
            int[] var04 = new int[3] { a, b, c };
            int[] var05 = new int[0];
            int[] var06 = new int[3];
            int[] var07 = new[] { a, b, c };
            int[] var08 = { };
            int[] var09 = { a, b, c };
            int[] var10 = Array.Empty<int>();

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

        void Consumer(int[] items) { }
        void ConsumerGeneric<T>(T[] items) { }

        int[] Property01 { get; } = new int[] { };
        int[] Property02 { get; } = new int[] { 1, 2, 3 };
        int[] Property03 { get; set; } = new int[0] { };
        int[] Property04 { get; set; } = new int[3] { 1, 2, 3 };
        int[] Property05 => new int[0];
        int[] Property06 => new int[3];
        int[] Property07 => new[] { 1, 2, 3 };
        int[] Property08 { get; } = { };
        int[] Property09 { get; set; } = { 1, 2, 3 };
        int[] Property10 { get; set; } = Array.Empty<int>();
    }

    public class GenericClass<T> where T : new()
    {
        T[] field01 = new T[] { };
        T[] field02 = new T[] { default, default(T), new() };
        T[] field03 = new T[0] { };
        T[] field04 = new T[3] { default, default(T), new() };
        T[] field05 = new T[0];
        T[] field06 = new T[3];
        T[] field07 = new[] { default, default(T), new() };
        T[] field08 = { };
        T[] field09 = { default, default(T), new() };
        T[] field10 = Array.Empty<T>();

        void Method(T a, T b, T c)
        {
            T[] var01 = new T[] { };
            T[] var02 = new T[] { a, b, c };
            T[] var03 = new T[0] { };
            T[] var04 = new T[3] { a, b, c };
            T[] var05 = new T[0];
            T[] var06 = new T[3];
            T[] var07 = new[] { a, b, c };
            T[] var08 = { };
            T[] var09 = { a, b, c };
            T[] var10 = Array.Empty<T>();

            Consumer(new T[] { });
            Consumer(new T[] { a, b, c });
            Consumer(new T[0] { });
            Consumer(new T[3] { a, b, c });
            Consumer(new T[0]);
            Consumer(new T[3]);
            Consumer(new[] { a, b, c });
            Consumer(Array.Empty<T>());
        }

        void Consumer(T[] items) { }

        T[] Property01 { get; } = new T[] { };
        T[] Property02 { get; } = new T[] { default, default(T), new() };
        T[] Property03 { get; set; } = new T[0] { };
        T[] Property04 { get; set; } = new T[3] { default, default(T), new() };
        T[] Property05 => new T[0];
        T[] Property06 => new T[3];
        T[] Property07 => new[] { default, default(T), new() };
        T[] Property08 { get; } = { };
        T[] Property09 { get; set; } = { default, default(T), new() };
        T[] Property10 { get; set; } = Array.Empty<T>();
    }

    internal class A { }
    internal class B(int x = 0) : A { }

    public class InferenceClass
    {
        A[] field1 = new B[] { };
        A[] field2 = new B[] { new(1), new(2), new(3) };
        A[] field3 = new B[0] { };
        A[] field4 = new B[3] { new B(1), new B(2), new B(3) };
        A[] field5 = new B[0];
        A[] field6 = new B[3];
        A[] field7 = new[] { new B(1), new B(2), new B(3) };
        A[] field8 = Array.Empty<B>();

        void Method(B a, B b, B c)
        {
            A[] var1 = new B[] { };
            A[] var2 = new B[] { a, b, c };
            A[] var3 = new B[0] { };
            A[] var4 = new B[3] { a, b, c };
            A[] var5 = new B[0];
            A[] var6 = new B[3];
            A[] var7 = new[] { a, b, c };
            A[] var8 = Array.Empty<B>();

            Consumer(new B[] { });
            Consumer(new B[] { a, b, c });
            Consumer(new B[0] { });
            Consumer(new B[3] { a, b, c });
            Consumer(new B[0]);
            Consumer(new B[3]);
            Consumer(new[] { a, b, c });
            Consumer(Array.Empty<B>());
        }

        void Consumer(A[] items) { }

        A[] Property1 { get; } = new B[] { };
        A[] Property2 { get; } = new B[] { new(1), new(2), new(3) };
        A[] Property3 { get; set; } = new B[0] { };
        A[] Property4 { get; set; } = new B[3] { new B(1), new B(2), new B(3) };
        A[] Property5 => new B[0];
        A[] Property6 => new B[3];
        A[] Property7 => new[] { new B(1), new B(2), new B(3) };
        A[] Property8 => Array.Empty<B>();
    }
}