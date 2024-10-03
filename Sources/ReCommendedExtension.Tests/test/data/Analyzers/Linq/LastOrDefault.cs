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
            var last1 = list.LastOrDefault();
            var last2 = list.LastOrDefault(1);
        }
        
        public void GenericMethod<T>(IList<T> list2, T fallback)
        {
            var last1 = list2.LastOrDefault();
            var last2 = list2.LastOrDefault(fallback);
        }
        
        public void ConstrainedGenericMethod<L>(L list3) where L : IList<int>
        {
            var last1 = list3.LastOrDefault();
            var last2 = list3.LastOrDefault(1);
        }
        
        public void Method(object list4)
        {
            var last1 = ((IList<int>)list4).LastOrDefault();
            var last2 = ((IList<int>)list4).LastOrDefault(1);
        }
        
        public void NonGenericMethod(List<int> list5)
        {
            var last1 = list5.LastOrDefault();
            var last2 = list5.LastOrDefault(1);
        }

        public void NonGenericMethod(int[] list6)
        {
            var last1 = list6.LastOrDefault();
            var last2 = list6.LastOrDefault(1);
        }

        public void NonGenericMethod(int[][] list7, int[] fallback)
        {
            var last1 = list7.LastOrDefault();
            var last2 = list7.LastOrDefault(fallback);
        }

        public void GenericMethod<T>(CustomList<T> list8, T fallback)
        {
            var last1 = list8.LastOrDefault();
            var last2 = list8.LastOrDefault(fallback);
        }

        public void NonGenericMethod(IReadOnlyList<int> list9)
        {
            var last1 = list9.LastOrDefault();
            var last2 = list9.LastOrDefault(2);
        }

        public void NonMatch(IList<int> list)
        {
            var last1 = list.LastOrDefault(x => x > 0);
            var last2 = list.LastOrDefault(x => x > 0, 1);
        }
    }

    public class MixedBehaviorCollections
    {
        public void Method(CustomListUniqueItems<int> list)
        {
            var last1 = list.LastOrDefault();
            var last2 = list.LastOrDefault(1);
            var last3 = list.LastOrDefault(x => x > 0);
            var last4 = list.LastOrDefault(x => x > 0, 1);
        }
    }

    public class DistinctCollections
    {
        public void NonGenericMethod(ISet<int> collection)
        {
            var last1 = collection.LastOrDefault();
            var last2 = collection.LastOrDefault(1);
            var last3 = collection.LastOrDefault(x => x > 0);
            var last4 = collection.LastOrDefault(x => x > 0, 1);
        }

        public void NonGenericMethod(IReadOnlySet<int> collection)
        {
            var last1 = collection.LastOrDefault();
            var last2 = collection.LastOrDefault(1);
            var last3 = collection.LastOrDefault(x => x > 0);
            var last4 = collection.LastOrDefault(x => x > 0, 1);
        }

        public void NonGenericMethod(Dictionary<int, string>.KeyCollection collection)
        {
            var last1 = collection.LastOrDefault();
            var last2 = collection.LastOrDefault(1);
            var last3 = collection.LastOrDefault(x => x > 0);
            var last4 = collection.LastOrDefault(x => x > 0, 1);
        }

        public void NonGenericMethod(HashSet<int> collection)
        {
            var last1 = collection.LastOrDefault();
            var last2 = collection.LastOrDefault(1);
            var last3 = collection.LastOrDefault(x => x > 0);
            var last4 = collection.LastOrDefault(x => x > 0, 1);
        }

        public void GenericMethod<T>(HashSet<T> collection, T fallback)
        {
            var last1 = collection.LastOrDefault();
            var last2 = collection.LastOrDefault(fallback);
            var last3 = collection.LastOrDefault(x => x.ToString() != "");
            var last4 = collection.LastOrDefault(x => x.ToString() != "", fallback);
        }

        public void ConstrainedGenericMethod<L>(L collection) where L : IReadOnlySet<int>
        {
            var last1 = collection.LastOrDefault();
            var last2 = collection.LastOrDefault(1);
            var last3 = collection.LastOrDefault(x => x > 0);
            var last4 = collection.LastOrDefault(x => x > 0, 1);
        }
    }
}