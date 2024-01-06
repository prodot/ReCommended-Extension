using System.Collections.Generic;

namespace Test
{
    class A { }
    class B : A { }

    public class NonGenericClass
    {
        int[] field01 = [0, 00, -0, 0x0, 0b0, default, default(int), new int()];
        int[] field02 = [0, 1, 2];
        int?[] field03 = [null, null as int?, default, default(int?), new int?()];
        int?[] field04 = [null, 0];

        IEnumerable<int> field11 = [0, 00, -0, 0x0, 0b0, default, default(int), new int()];
        IEnumerable<int> field12 = [0, 1, 2];
        IEnumerable<int?> field13 = [null, null as int?, default, default(int?), new int?()];
        IEnumerable<int?> field14 = [null, 0];

        IReadOnlyCollection<int> field21 = [0, 00, -0, 0x0, 0b0, default, default(int), new int()];
        IReadOnlyCollection<int> field22 = [0, 1, 2];
        IReadOnlyCollection<int?> field23 = [null, null as int?, default, default(int?), new int?()];
        IReadOnlyCollection<int?> field24 = [null, 0];

        IReadOnlyList<int> field31 = [0, 00, -0, 0x0, 0b0, default, default(int), new int()];
        IReadOnlyList<int> field32 = [0, 1, 2];
        IReadOnlyList<int?> field33 = [null, null as int?, default, default(int?), new int?()];
        IReadOnlyList<int?> field34 = [null, 0];

        ICollection<int> field41 = [0, 00, -0, 0x0, 0b0, default, default(int), new int()];
        ICollection<int> field42 = [0, 1, 2];
        ICollection<int?> field43 = [null, null as int?, default, default(int?), new int?()];
        ICollection<int?> field44 = [null, 0];

        IList<int> field51 = [0, 00, -0, 0x0, 0b0, default, default(int), new int()];
        IList<int> field52 = [0, 1, 2];
        IList<int?> field53 = [null, null as int?, default, default(int?), new int?()];
        IList<int?> field54 = [null, 0];

        IEnumerable<A?> field61 = [default(B), default(B)];
    }

    public class GenericClass<T> where T : new()
    {
        T[] field01 = [default, default(T)];
        T[] field02 = [default, new T()];

        IEnumerable<T> field11 = [default, default(T)];
        IEnumerable<T> field12 = [default, new T()];

        IList<T> field51 = [default, default(T)];
        IList<T> field52 = [default, new T()];
    }

    public class GenericClass_ValueType<T> where T : struct
    {
        T[] field01 = [default, default(T)];
        T[] field02 = [default, default(T), new T()];
        T?[] field03 = [null, null as T?, default, default(T?), new T?()];
        T?[] field04 = [null, default(T)];

        IEnumerable<T> field11 = [default, default(T)];
        IEnumerable<T> field12 = [default, default(T), new T()];
        IEnumerable<T?> field13 = [null, null as T?, default, default(T?), new T?()];
        IEnumerable<T?> field14 = [null, default(T)];

        IList<T> field51 = [default, default(T)];
        IList<T> field52 = [default, default(T), new T()];
        IList<T?> field53 = [null, null as T?, default, default(T?), new T?()];
        IList<T?> field54 = [null, default(T)];
    }

    public class GenericClass_ReferenceType<T> where T : class
    {
        T?[] field03 = [null, null as T, default, default(T)];

        IEnumerable<T?> field13 = [null, null as T, default, default(T)];

        IList<T?> field53 = [null, null as T, default, default(T)];
    }
}