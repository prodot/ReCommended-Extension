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
        ICollection<string?> Property8 => Array.Empty<int>();
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
}