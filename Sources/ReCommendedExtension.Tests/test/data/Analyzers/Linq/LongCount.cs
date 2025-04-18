using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    class CustomCollection<T> : ICollection<T>
    {
        public void Add(T item) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        public bool Contains(T item) => throw new NotImplementedException();

        public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(T item) => throw new NotImplementedException();

        public int Count { get; }

        public bool IsReadOnly { get; }

        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Strings
    {
        public void Method(string collection)
        {
            var count = collection.LongCount();
        }

        public void NonMatch(string collection)
        {
            var count = collection.LongCount(c => c == 'A');
        }
    }

    public class Arrays
    {
        public void Method(int[] collection)
        {
            var count = collection.LongCount();
        }

        public void Method(int[][] collection)
        {
            var count = collection.LongCount();
        }

        public void NonMatch(int[] collection)
        {
            var count = collection.LongCount(c => c == 'A');
        }
    }

    public class Collections
    {
        public void NonGenericMethod(ICollection<int> collection)
        {
            var count = collection.LongCount();
        }

        public void GenericMethod<T>(ICollection<T> collection)
        {
            var count = collection.LongCount();
        }

        public void ConstrainedGenericMethod<L>(L collection) where L : ICollection<int>
        {
            var count = collection.LongCount();
        }

        public void Method(object collection)
        {
            var count = ((ICollection<int>)collection).LongCount();
        }
        
        public void NonGenericMethod(List<int> collection)
        {
            var count = collection.LongCount();
        }

        public void NonGenericMethod(HashSet<int> collection)
        {
            var count = collection.LongCount();
        }

        public void GenericMethod<T>(CustomCollection<T> collection)
        {
            var count = collection.LongCount();
        }

        public void NonGenericMethod(IReadOnlyCollection<int> collection)
        {
            var count = collection.LongCount();
        }

        public void NonMatch(ICollection<int> collection)
        {
            var count = collection.LongCount(x => x > 0);
        }
    }
}