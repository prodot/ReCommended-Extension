using System;
using System.Linq;

namespace NonTargetTyped
{
    public class NonGenericClass
    {
        void Method(int a, int b, int c)
        {
            var var1 = new int[] { };
            var var2 = new int[] { a, b, c };
            var var3 = new int[0] { };
            var var4 = new int[3] { a, b, c };
            var var5 = new int[0];
            var var6 = new int[3];
            var var7 = new[] { a, b, c };
            var var8 = Array.Empty<int>();

            IEnumerable<int> var9 = Array.Empty<int>().AsEnumerable();
        }
    }

    public class GenericClass<T>
    {
        void Method(T a, T b, T c)
        {
            var var1 = new T[] { };
            var var2 = new T[] { a, b, c };
            var var3 = new T[0] { };
            var var4 = new T[3] { a, b, c };
            var var5 = new T[0];
            var var6 = new T[3];
            var var7 = new[] { a, b, c };
            var var8 = Array.Empty<T>();

            IEnumerable<T> var9 = Array.Empty<T>().AsEnumerable();
        }
    }
}