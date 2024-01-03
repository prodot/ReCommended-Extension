using System;
using System.Collections.Generic;

namespace TargetList
{
    public class NonGenericClass
    {
        IList<int> field1 = new int[] { };
        IList<int> field2 = new int[] { 1, 2, 3 };
        IList<int> field3 = new int[0] { };
        IList<int> field4 = new int[3] { 1, 2, 3 };
        IList<int> field5 = new int[0];
        IList<int> field6 = new int[3];
        IList<int> field7 = new[] { 1, 2, 3 };

        void Method(int a, int b, int c)
        {
            IList<int> var1 = new int[] { };
            IList<int> var2 = new int[] { a, b, c };
            IList<int> var3 = new int[0] { };
            IList<int> var4 = new int[3] { a, b, c };
            IList<int> var5 = new int[0];
            IList<int> var6 = new int[3];
            IList<int> var7 = new[] { a, b, c };

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

        void Consumer(IList<int> items) { }
        void ConsumerGeneric<T>(IList<T> items) { }

        IList<string?> Property1 { get; } = new string?[] { };
        IList<string?> Property2 { get; } = new string?[] { "one", "two", "three" };
        IList<string?> Property3 { get; set; } = new string?[0] { };
        IList<string?> Property4 { get; set; } = new string?[3] { "one", "two", "three" };
        IList<string?> Property5 => new string?[0];
        IList<string?> Property6 => new string?[3];
        IList<string?> Property7 => new[] { "one", "two", "three", null };
    }

    public class GenericClass<T> where T : new()
    {
        IList<T> field1 = new T[] { };
        IList<T> field2 = new T[] { default, default(T), new() };
        IList<T> field3 = new T[0] { };
        IList<T> field4 = new T[3] { default, default(T), new() };
        IList<T> field5 = new T[0];
        IList<T> field6 = new T[3];
        IList<T> field7 = new[] { default, default(T), new() };

        void Method(T a, T b, T c)
        {
            IList<T> var1 = new T[] { };
            IList<T> var2 = new T[] { a, b, c };
            IList<T> var3 = new T[0] { };
            IList<T> var4 = new T[3] { a, b, c };
            IList<T> var5 = new T[0];
            IList<T> var6 = new T[3];
            IList<T> var7 = new[] { a, b, c };

            Consumer(new T[] { });
            Consumer(new T[] { a, b, c });
            Consumer(new T[0] { });
            Consumer(new T[3] { a, b, c });
            Consumer(new T[0]);
            Consumer(new T[3]);
            Consumer(new[] { a, b, c });
        }

        void Consumer(IList<T> items) { }

        IList<T> Property1 { get; } = new T[] { };
        IList<T> Property2 { get; } = new T[] { default, default(T), new() };
        IList<T> Property3 { get; set; } = new T[0] { };
        IList<T> Property4 { get; set; } = new T[3] { default, default(T), new() };
        IList<T> Property5 => new T[0];
        IList<T> Property6 => new T[3];
        IList<T> Property7 => new[] { default, default(T), new() };
    }
}