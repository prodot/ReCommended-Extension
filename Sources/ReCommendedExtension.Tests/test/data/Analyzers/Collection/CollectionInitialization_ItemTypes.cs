using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    class A { }
    class B : A { }

    public class NonGenericClass
    {
        void Arrays(A a, B b)
        {
            IEnumerable<object> var2 = new[] { "one", "two", "three" };

            IEnumerable<A> var3 = new[] { b, b };
            IReadOnlyCollection<A> var4 = new[] { b, b };
            IReadOnlyList<A> var5 = new[] { b, b };

            ICollection<A> var6 = new[] { b, b };
            IList<A> var7 = new[] { b, b };

            A[] var9 = new[] { b, b };
        }

        void ArraysEmpty(A a, B b)
        {
            IEnumerable<object> var2 = new string[] { };

            IEnumerable<A> var3 = new B[] { };
            IReadOnlyCollection<A> var4 = new B[] { };
            IReadOnlyList<A> var5 = new B[] { };

            ICollection<A> var6 = new B[] { };
            IList<A> var7 = new B[] { };

            A[] var9 = new B[] { };
        }

        void ArraysEmpty2(A a, B b)
        {
            IEnumerable<object> var2 = Array.Empty<string>();

            IEnumerable<A> var3 = Array.Empty<B>();
            IReadOnlyCollection<A> var4 = Array.Empty<B>();
            IReadOnlyList<A> var5 = Array.Empty<B>();

            ICollection<A> var6 = Array.Empty<B>();
            IList<A> var7 = Array.Empty<B>();

            A[] var9 = Array.Empty<B>();
        }

        void EnumerableEmpty()
        {
            IEnumerable<int> var1 = Enumerable.Empty<int>();
            IEnumerable<object> var2 = Enumerable.Empty<string>();
        }

        void Lists(A a, B b)
        {
            IEnumerable<object> var1 = new List<string>() { "one", "two", "three" };

            IEnumerable<A> var2 = new List<B>() { b, b };
            IReadOnlyCollection<A> var3 = new List<B>() { b, b };
            IReadOnlyList<A> var4 = new List<B>() { b, b };
        }

        void ListsEmpty(A a, B b)
        {
            IEnumerable<object> var1 = new List<string>();

            IEnumerable<A> var2 = new List<B>();
            IReadOnlyCollection<A> var3 = new List<B>();
            IReadOnlyList<A> var4 = new List<B>();
        }

        void HashSets(A a, B b)
        {
            IEnumerable<object> var1 = new HashSet<string>() { "one", "two", "three" };

            IEnumerable<A> var2 = new HashSet<B>() { b, b };
            IReadOnlyCollection<A> var3 = new HashSet<B>() { b, b };
        }

        void HashSetsEmpty(A a, B b)
        {
            IEnumerable<object> var1 = new HashSet<string>();

            IEnumerable<A> var2 = new HashSet<B>();
            IReadOnlyCollection<A> var3 = new HashSet<B>();
        }

        void DictionariesEmpty()
        {
            IDictionary<int, string> var1 = new Dictionary<int, string>();
        }
    }
}