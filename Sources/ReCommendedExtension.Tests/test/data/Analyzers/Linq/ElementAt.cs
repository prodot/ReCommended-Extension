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
            var third = list.ElementAt(2);
            var secondFromEnd = list.ElementAt(^2);
        }
        
        public void GenericMethod<T>(IList<T> list2)
        {
            var third = list2.ElementAt(2);
            var secondFromEnd = list2.ElementAt(^2);
        }
        
        public void ConstrainedGenericMethod<L>(L list3) where L : IList<int>
        {
            var third = list3.ElementAt(2);
            var secondFromEnd = list3.ElementAt(^2);
        }
        
        public void Method(object list4)
        {
            var third = ((IList<int>)list4).ElementAt(2);
            var secondFromEnd = ((IList<int>)list4).ElementAt(^2);
        }
        
        public void NonGenericMethod(List<int> list5)
        {
            var third = list5.ElementAt(2);
            var secondFromEnd = list5.ElementAt(^2);
        }

        public void NonGenericMethod(int[] list6)
        {
            var third = list6.ElementAt(2);
            var secondFromEnd = list6.ElementAt(^2);
        }

        public void NonGenericMethod(int[][] list7)
        {
            var third = list7.ElementAt(2);
            var secondFromEnd = list7.ElementAt(^2);
        }

        public void GenericMethod<T>(CustomList<T> list8)
        {
            var third = list8.ElementAt(2);
            var secondFromEnd = list8.ElementAt(^2);
        }

        public void NonGenericMethod(IReadOnlyList<int> list9)
        {
            var third = list9.ElementAt(2);
            var secondFromEnd = list9.ElementAt(^2);
        }

        public void NonGenericMethod(string text)
        {
            var third = text.ElementAt(2);
            var secondFromEnd = text.ElementAt(^2);
        }

        public void NonAccessibleIndexers(OrderedDictionary<int, string> orderedDictionary)
        {
            var third = orderedDictionary.ElementAt(2);
            var secondFromEnd = orderedDictionary.ElementAt(^2);
        }
    }

    public class ConstrainedGenericClass<L> where L : IList<int>
    {
        public void ConstrainedGenericMethod(L list3)
        {
            var third = list3.ElementAt(2);
            var secondFromEnd = list3.ElementAt(^2);
        }
    }

    public class MixedBehaviorCollections
    {
        public void Method(CustomListUniqueItems<int> list)
        {
            var third = list.ElementAt(2);
            var secondFromEnd = list.ElementAt(^2);
        }
    }

    public class DistinctCollections
    {
        public void NonGenericMethod(ISet<int> collection)
        {
            var third = collection.ElementAt(2);
            var secondFromEnd = collection.ElementAt(^2);
        }

        public void NonGenericMethod(IReadOnlySet<int> collection)
        {
            var third = collection.ElementAt(2);
            var secondFromEnd = collection.ElementAt(^2);
        }

        public void NonGenericMethod(Dictionary<int, string>.KeyCollection collection)
        {
            var third = collection.ElementAt(2);
            var secondFromEnd = collection.ElementAt(^2);
        }

        public void NonGenericMethod(HashSet<int> collection)
        {
            var third = collection.ElementAt(2);
            var secondFromEnd = collection.ElementAt(^2);
        }

        public void GenericMethod<T>(HashSet<T> collection)
        {
            var third = collection.ElementAt(2);
            var secondFromEnd = collection.ElementAt(^2);
        }

        public void ConstrainedGenericMethod<L>(L collection) where L : IReadOnlySet<int>
        {
            var third = collection.ElementAt(2);
            var secondFromEnd = collection.ElementAt(^2);
        }
    }
}