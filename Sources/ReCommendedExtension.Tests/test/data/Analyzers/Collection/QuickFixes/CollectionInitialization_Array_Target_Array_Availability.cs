using System;

namespace TargetArray
{
    public class NonGenericClass
    {
        int[] field1 = new int[] { };
        int[] field2 = new int[] { 1, 2, 3 };
        int[] field3 = new int[0] { };
        int[] field4 = new int[3] { 1, 2, 3 };
        int[] field6 = new int[3];
        int[] field7 = new[] { 1, 2, 3 };
        int[] field8 = { };
        int[] field9 = { 1, 2, 3 };

        void Method(int a, int b, int c)
        {
            int[] var1 = new int[] { };
            int[] var2 = new int[] { a, b, c };
            int[] var3 = new int[0] { };
            int[] var4 = new int[3] { a, b, c };
            int[] var6 = new int[3];
            int[] var7 = new[] { a, b, c };
            int[] var8 = { };
            int[] var9 = { a, b, c };

            Consumer(new int[] { });
            Consumer(new int[] { a, b, c });
            Consumer(new int[0] { });
            Consumer(new int[3] { a, b, c });
            Consumer(new int[0]);
            Consumer(new int[3]);
            Consumer(new[] { a, b, c });

            ConsumerGeneric(new int[] { });
            ConsumerGeneric(new int[] { a, b, c });
            ConsumerGeneric<int>(new int[0] { });
            ConsumerGeneric(new int[3] { a, b, c });
            ConsumerGeneric(new int[3]);
            ConsumerGeneric(new[] { a, b, c });
        }

        void Consumer(int[] items) { }
        void ConsumerGeneric<T>(T[] items) { }

        int[] Property1 { get; } = new int[] { };
        int[] Property2 { get; } = new int[] { 1, 2, 3 };
        int[] Property3 { get; set; } = new int[0] { };
        int[] Property4 { get; set; } = new int[3] { 1, 2, 3 };
        int[] Property6 => new int[3];
        int[] Property7 => new[] { 1, 2, 3 };
        int[] Property8 { get; } = { };
        int[] Property9 { get; set; } = { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        T[] field1 = new T[] { };
        T[] field2 = new T[] { default, default(T), new() };
        T[] field3 = new T[0] { };
        T[] field4 = new T[3] { default, default(T), new() };
        T[] field6 = new T[3];
        T[] field7 = new[] { default, default(T), new() };
        T[] field8 = { };
        T[] field9 = { default, default(T), new() };

        void Method(T a, T b, T c)
        {
            T[] var1 = new T[] { };
            T[] var2 = new T[] { a, b, c };
            T[] var3 = new T[0] { };
            T[] var4 = new T[3] { a, b, c };
            T[] var6 = new T[3];
            T[] var7 = new[] { a, b, c };
            T[] var8 = { };
            T[] var9 = { a, b, c };

            Consumer(new T[] { });
            Consumer(new T[] { a, b, c });
            Consumer(new T[0] { });
            Consumer(new T[3] { a, b, c });
            Consumer(new T[3]);
            Consumer(new[] { a, b, c });
        }

        void Consumer(T[] items) { }

        T[] Property1 { get; } = new T[] { };
        T[] Property2 { get; } = new T[] { default, default(T), new() };
        T[] Property3 { get; set; } = new T[0] { };
        T[] Property4 { get; set; } = new T[3] { default, default(T), new() };
        T[] Property6 => new T[3];
        T[] Property7 => new[] { default, default(T), new() };
        T[] Property8 { get; } = { };
        T[] Property9 { get; set; } = { default, default(T), new() };
    }
}