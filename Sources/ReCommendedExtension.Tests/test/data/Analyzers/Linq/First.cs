using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    class CustomList<T> : IList<T>
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

    class CustomListUniqueItems<T> : CustomList<T>, ISet<T>
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
            var first = list.First();
        }
        
        public void GenericMethod<T>(IList<T> list2)
        {
            var first = list2.First();
        }
        
        public void ConstrainedGenericMethod<L>(L list3) where L : IList<int>
        {
            var first = list3.First();
        }
        
        public void Method(object list4)
        {
            var first = ((IList<int>)list4).First();
        }
        
        public void NonGenericMethod(List<int> list5)
        {
            var first = list5.First();
        }

        public void NonGenericMethod(int[] list6)
        {
            var first = list6.First();
        }

        public void NonGenericMethod(int[][] list7)
        {
            var first = list7.First();
        }

        public void GenericMethod<T>(CustomList<T> list8)
        {
            var first = list8.First();
        }

        public void NonGenericMethod(IReadOnlyList<int> list9)
        {
            var first = list9.First();
        }

        public void NonGenericMethod(string text)
        {
            var first = text.First();
        }

        public void NonMatch(IList<int> list)
        {
            var first = list.First(x => x > 0);
        }

        public void NonAccessibleIndexers(OrderedDictionary<int, string> orderedDictionary)
        {
            var third = orderedDictionary.First();
            var secondFromEnd = orderedDictionary.First();
        }
    }

    public class ConstrainedGenericClass<L> where L : IList<int>
    {
        public void ConstrainedGenericMethod(L list3)
        {
            var third = list3.First();
            var secondFromEnd = list3.First();
        }
    }

    public class MixedBehaviorCollections
    {
        public void Method(CustomListUniqueItems<int> list)
        {
            var first1 = list.First();
            var first2 = list.First(x => x > 0);
        }
    }

    public class DistinctCollections
    {
        public void NonGenericMethod(ISet<int> collection)
        {
            var first1 = collection.First();
            var first2 = collection.First(x => x > 0);
        }

        public void NonGenericMethod(IReadOnlySet<int> collection)
        {
            var first1 = collection.First();
            var first2 = collection.First(x => x > 0);
        }

        public void NonGenericMethod(Dictionary<int, string>.KeyCollection collection)
        {
            var first1 = collection.First();
            var first2 = collection.First(x => x > 0);
        }

        public void NonGenericMethod(HashSet<int> collection)
        {
            var first1 = collection.First();
            var first2 = collection.First(x => x > 0);
        }

        public void GenericMethod<T>(HashSet<T> collection)
        {
            var first1 = collection.First();
            var first2 = collection.First(x => x.ToString() != "");
        }

        public void ConstrainedGenericMethod<L>(L collection) where L : IReadOnlySet<int>
        {
            var first1 = collection.First();
            var first2 = collection.First(x => x > 0);
        }
    }
}