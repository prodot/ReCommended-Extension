using System;
using System.Collections.Generic;

namespace TargetReadOnlyList
{
    public class NonGenericClass
    {
        IReadOnlyList<int> field1 = new int[] { };
        IReadOnlyList<int> field2 = new int[] { 1, 2, 3 };
        IReadOnlyList<int> field3 = new int[0] { };
        IReadOnlyList<int> field4 = new int[3] { 1, 2, 3 };
        IReadOnlyList<int> field5 = new int[0];
        IReadOnlyList<int> field6 = new int[3];
        IReadOnlyList<int> field7 = new[] { 1, 2, 3 };

        void Method(int a, int b, int c)
        {
            IReadOnlyList<int> var1 = new int[] { };
            IReadOnlyList<int> var2 = new int[] { a, b, c };
            IReadOnlyList<int> var3 = new int[0] { };
            IReadOnlyList<int> var4 = new int[3] { a, b, c };
            IReadOnlyList<int> var5 = new int[0];
            IReadOnlyList<int> var6 = new int[3];
            IReadOnlyList<int> var7 = new[] { a, b, c };

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
            ConsumerGeneric(new int[0]);
            ConsumerGeneric(new int[3]);
            ConsumerGeneric(new[] { a, b, c });
        }

        void Consumer(IReadOnlyList<int> items) { }
        void ConsumerGeneric<T>(IReadOnlyList<T> items) { }

        IReadOnlyList<int> Property1 { get; } = new int[] { };
        IReadOnlyList<int> Property2 { get; } = new int[] { 1, 2, 3 };
        IReadOnlyList<int> Property3 { get; set; } = new int[0] { };
        IReadOnlyList<int> Property4 { get; set; } = new int[3] { 1, 2, 3 };
        IReadOnlyList<int> Property5 => new int[0];
        IReadOnlyList<int> Property6 => new int[3];
        IReadOnlyList<int> Property7 => new[] { 1, 2, 3 };
    }

    public class GenericClass<T> where T : new()
    {
        IReadOnlyList<T> field1 = new T[] { };
        IReadOnlyList<T> field2 = new T[] { default, default(T), new() };
        IReadOnlyList<T> field3 = new T[0] { };
        IReadOnlyList<T> field4 = new T[3] { default, default(T), new() };
        IReadOnlyList<T> field5 = new T[0];
        IReadOnlyList<T> field6 = new T[3];
        IReadOnlyList<T> field7 = new[] { default, default(T), new() };

        void Method(T a, T b, T c)
        {
            IReadOnlyList<T> var1 = new T[] { };
            IReadOnlyList<T> var2 = new T[] { a, b, c };
            IReadOnlyList<T> var3 = new T[0] { };
            IReadOnlyList<T> var4 = new T[3] { a, b, c };
            IReadOnlyList<T> var5 = new T[0];
            IReadOnlyList<T> var6 = new T[3];
            IReadOnlyList<T> var7 = new[] { a, b, c };

            Consumer(new T[] { });
            Consumer(new T[] { a, b, c });
            Consumer(new T[0] { });
            Consumer(new T[3] { a, b, c });
            Consumer(new T[0]);
            Consumer(new T[3]);
            Consumer(new[] { a, b, c });
        }

        void Consumer(IReadOnlyList<T> items) { }

        IReadOnlyList<T> Property1 { get; } = new T[] { };
        IReadOnlyList<T> Property2 { get; } = new T[] { default, default(T), new() };
        IReadOnlyList<T> Property3 { get; set; } = new T[0] { };
        IReadOnlyList<T> Property4 { get; set; } = new T[3] { default, default(T), new() };
        IReadOnlyList<T> Property5 => new T[0];
        IReadOnlyList<T> Property6 => new T[3];
        IReadOnlyList<T> Property7 => new[] { default, default(T), new() };
    }
}