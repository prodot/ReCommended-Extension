using System;
using System.Collections.Generic;

namespace Test
{
    class A { }
    class B : A { }
    class C { }

    public class NonGenericClass
    {
        void Arrays(A a, B b)
        {
            IEnumerable<object> var1 = new[] { 1, 2, 3 };
            IEnumerable<object> var2 = new[] { "one", "two", "three" };

            IEnumerable<A> var3 = new[] { b, b };
            IReadOnlyCollection<A> var4 = new[] { b, b };
            IReadOnlyList<A> var5 = new[] { b, b };

            ICollection<A> var6 = new[] { b, b };
            IList<A> var7 = new[] { b, b };

            IList<C> var8 = new[] { b, b };

            A[] var9 = new[] { b, b };
        }

        void ArraysEmpty(A a, B b)
        {
            IEnumerable<object> var1 = new int[] { };
            IEnumerable<object> var2 = new string[] { };

            IEnumerable<A> var3 = new B[] { };
            IReadOnlyCollection<A> var4 = new B[] { };
            IReadOnlyList<A> var5 = new B[] { };

            ICollection<A> var6 = new B[] { };
            IList<A> var7 = new B[] { };

            IList<C> var8 = new B[] { };

            A[] var9 = new B[] { };
        }

        void Lists(A a, B b)
        {
            IEnumerable<object> var1 = new List<int>() { 1, 2, 3 };
            IEnumerable<object> var2 = new List<string>() { "one", "two", "three" };

            IEnumerable<A> var2 = new List<B>() { b, b };
            IReadOnlyCollection<A> var3 = new List<B>() { b, b };
            IReadOnlyList<A> var4 = new List<B>() { b, b };
            
            ICollection<A> var6 = new List<B>() { b, b };
            IList<A> var7 = new List<B>() { b, b };

            IList<C> var8 = new List<B> { b, b };

            List<A> var9 = new List<B>() { b, b };
        }

        void ListsEmpty(A a, B b)
        {
            IEnumerable<object> var1 = new List<int>();
            IEnumerable<object> var2 = new List<string>();

            IEnumerable<A> var2 = new List<B>();
            IReadOnlyCollection<A> var3 = new List<B>();
            IReadOnlyList<A> var4 = new List<B>();

            ICollection<A> var6 = new List<B>();
            IList<A> var7 = new List<B>();

            IList<C> var8 = new List<B>;

            List<A> var9 = new List<B>();
        }

        void DictionariesEmpty()
        {
            IDictionary<int, string> var1 = new Dictionary<int, string>();
        }
    }
}