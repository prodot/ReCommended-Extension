using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class CustomList<T> : IList<T>
    {
        public int IndexOf(T item) => throw new NotImplementedException();

        public void Insert(int index, T item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        public T this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Add(T item) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        public bool Contains(T item) => throw new NotImplementedException();

        public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(T item) => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class CustomListUniqueItems<T> : CustomList<T>, ISet<T>
    {
        public bool Add(T item) => throw new NotImplementedException();

        public void UnionWith(IEnumerable<T> other) => throw new NotImplementedException();

        public void IntersectWith(IEnumerable<T> other) => throw new NotImplementedException();

        public void ExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

        public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

        public bool IsSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

        public bool IsSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

        public bool IsProperSupersetOf(IEnumerable<T> other) => throw new NotImplementedException();

        public bool IsProperSubsetOf(IEnumerable<T> other) => throw new NotImplementedException();

        public bool Overlaps(IEnumerable<T> other) => throw new NotImplementedException();

        public bool SetEquals(IEnumerable<T> other) => throw new NotImplementedException();
    }

    public class IndexableCollections
    {
        public void NonGenericMethod(IList<int> list)
        {
            var last = list.Last();
        }

        public void GenericMethod<T>(IList<T> list2)
        {
            var last = list2.Last();
        }

        public void ConstrainedGenericMethod<L>(L list3) where L : IList<int>
        {
            var last = list3.Last();
        }

        public void Method(object list4)
        {
            var last = ((IList<int>)list4).Last();
        }

        public void NonGenericMethod(List<int> list5)
        {
            var last = list5.Last();
        }

        public void NonGenericMethod(int[] list6)
        {
            var last = list6.Last();
        }

        public void NonGenericMethod(int[][] list7)
        {
            var last = list7.Last();
        }

        public void GenericMethod<T>(CustomList<T> list8)
        {
            var last = list8.Last();
        }

        public void NonGenericMethod(IReadOnlyList<int> list9)
        {
            var last = list9.Last();
        }

        public void NonGenericMethod(string text)
        {
            var last = text.Last();
        }

        public void NonMatch(IList<int> list)
        {
            var last = list.Last(x => x > 0);
        }

        public void NonAccessibleIndexers(OrderedDictionary<int, string> orderedDictionary)
        {
            var third = orderedDictionary.Last();
            var secondFromEnd = orderedDictionary.Last();
        }
    }

    public class ConstrainedGenericClass<L> where L : IList<int>
    {
        public void ConstrainedGenericMethod(L list3)
        {
            var third = list3.Last();
            var secondFromEnd = list3.Last();
        }
    }

    public class MixedBehaviorCollections
    {
        public void Method(CustomListUniqueItems<int> list)
        {
            var last1 = list.Last();
            var last2 = list.Last(x => x > 0);
        }
    }

    public class DistinctCollections
    {
        public void NonGenericMethod(ISet<int> collection)
        {
            var last1 = collection.Last();
            var last2 = collection.Last(x => x > 0);
        }

        public void NonGenericMethod(IReadOnlySet<int> collection)
        {
            var last1 = collection.Last();
            var last2 = collection.Last(x => x > 0);
        }

        public void NonGenericMethod(Dictionary<int, string>.KeyCollection collection)
        {
            var last1 = collection.Last();
            var last2 = collection.Last(x => x > 0);
        }

        public void NonGenericMethod(HashSet<int> collection)
        {
            var last1 = collection.Last();
            var last2 = collection.Last(x => x > 0);
        }

        public void GenericMethod<T>(HashSet<T> collection)
        {
            var last1 = collection.Last();
            var last2 = collection.Last(x => x.ToString() != "");
        }

        public void ConstrainedGenericMethod<L>(L collection) where L : IReadOnlySet<int>
        {
            var last1 = collection.Last();
            var last2 = collection.Last(x => x > 0);
        }
    }
}