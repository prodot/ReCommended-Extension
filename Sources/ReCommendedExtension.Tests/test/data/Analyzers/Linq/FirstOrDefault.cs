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
            var first1 = list.FirstOrDefault();
            var first2 = list.FirstOrDefault(1);
        }
        
        public void GenericMethod<T>(IList<T> list2, T fallback)
        {
            var first1 = list2.FirstOrDefault();
            var first2 = list2.FirstOrDefault(fallback);
        }
        
        public void ConstrainedGenericMethod<L>(L list3) where L : IList<int>
        {
            var first1 = list3.FirstOrDefault();
            var first2 = list3.FirstOrDefault(1);
        }
        
        public void Method(object list4)
        {
            var first1 = ((IList<int>)list4).FirstOrDefault();
            var first2 = ((IList<int>)list4).FirstOrDefault(1);
        }
        
        public void NonGenericMethod(List<int> list5)
        {
            var first1 = list5.FirstOrDefault();
            var first2 = list5.FirstOrDefault(1);
        }

        public void NonGenericMethod(int[] list6)
        {
            var first1 = list6.FirstOrDefault();
            var first2 = list6.FirstOrDefault(1);
        }

        public void NonGenericMethod(int[][] list7, int[] fallback)
        {
            var first1 = list7.FirstOrDefault();
            var first2 = list7.FirstOrDefault(fallback);
        }

        public void GenericMethod<T>(CustomList<T> list8, T fallback)
        {
            var first1 = list8.FirstOrDefault();
            var first2 = list8.FirstOrDefault(fallback);
        }

        public void NonGenericMethod(IReadOnlyList<int> list9)
        {
            var first1 = list9.FirstOrDefault();
            var first2 = list9.FirstOrDefault(1);
        }

        public void NonGenericMethod(string text)
        {
            var first1 = text.FirstOrDefault();
            var first2 = text.FirstOrDefault('a');
        }

        public void NonMatch(IList<int> list)
        {
            var first1 = list.FirstOrDefault(x => x > 0);
            var first2 = list.FirstOrDefault(x => x > 0, 1);
        }

        public void NonMatch2(IList<int>? list)
        {
            var first1 = list?.FirstOrDefault();
            var first2 = list?.FirstOrDefault(1);
        }

        public void NonAccessibleIndexers(OrderedDictionary<int, string> orderedDictionary)
        {
            var third = orderedDictionary.FirstOrDefault();
            var secondFromEnd = orderedDictionary.FirstOrDefault();
        }
    }

    public class ConstrainedGenericClass<L> where L : IList<int>
    {
        public void ConstrainedGenericMethod(L list3)
        {
            var third = list3.FirstOrDefault();
            var secondFromEnd = list3.FirstOrDefault();
        }
    }

    public class MixedBehaviorCollections
    {
        public void Method(CustomListUniqueItems<int> list)
        {
            var first1 = list.FirstOrDefault();
            var first2 = list.FirstOrDefault(1);
            var first3 = list.FirstOrDefault(x => x > 0);
            var first4 = list.FirstOrDefault(x => x > 0, 1);
        }
    }

    public class DistinctCollections
    {
        public void NonGenericMethod(ISet<int> collection)
        {
            var first1 = collection.FirstOrDefault();
            var first2 = collection.FirstOrDefault(1);
            var first3 = collection.FirstOrDefault(x => x > 0);
            var first4 = collection.FirstOrDefault(x => x > 0, 1);
        }

        public void NonGenericMethod(IReadOnlySet<int> collection)
        {
            var first1 = collection.FirstOrDefault();
            var first2 = collection.FirstOrDefault(1);
            var first3 = collection.FirstOrDefault(x => x > 0);
            var first4 = collection.FirstOrDefault(x => x > 0, 1);
        }

        public void NonGenericMethod(Dictionary<int, string>.KeyCollection collection)
        {
            var first1 = collection.FirstOrDefault();
            var first2 = collection.FirstOrDefault(1);
            var first3 = collection.FirstOrDefault(x => x > 0);
            var first4 = collection.FirstOrDefault(x => x > 0, 1);
        }

        public void NonGenericMethod(HashSet<int> collection)
        {
            var first1 = collection.FirstOrDefault();
            var first2 = collection.FirstOrDefault(1);
            var first3 = collection.FirstOrDefault(x => x > 0);
            var first4 = collection.FirstOrDefault(x => x > 0, 1);
        }

        public void GenericMethod<T>(HashSet<T> collection, T fallback)
        {
            var first1 = collection.FirstOrDefault();
            var first2 = collection.FirstOrDefault(fallback);
            var first3 = collection.FirstOrDefault(x => x.ToString() != "");
            var first4 = collection.FirstOrDefault(x => x.ToString() != "", fallback);
        }

        public void ConstrainedGenericMethod<L>(L collection) where L : IReadOnlySet<int>
        {
            var first1 = collection.FirstOrDefault();
            var first2 = collection.FirstOrDefault(1);
            var first3 = collection.FirstOrDefault(x => x > 0);
            var first4 = collection.FirstOrDefault(x => x > 0, 1);
        }
    }
}