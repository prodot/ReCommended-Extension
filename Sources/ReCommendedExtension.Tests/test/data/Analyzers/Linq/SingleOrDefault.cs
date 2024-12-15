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

    public class IndexableCollections
    {
        public void NonGenericMethod(IList<int> list)
        {
            var single1 = list.SingleOrDefault();
            var single2 = list.SingleOrDefault(1);
        }
        
        public void GenericMethod<T>(IList<T> list2, T fallback)
        {
            var single1 = list2.SingleOrDefault();
            var single2 = list2.SingleOrDefault(fallback);
        }
        
        public void ConstrainedGenericMethod<L>(L list3) where L : IList<int>
        {
            var single1 = list3.SingleOrDefault();
            var single2 = list3.SingleOrDefault(1);
        }
        
        public void Method(object list4)
        {
            var single1 = ((IList<int>)list4).SingleOrDefault();
            var single2 = ((IList<int>)list4).SingleOrDefault(1);
        }
        
        public void NonGenericMethod(List<int> list5)
        {
            var single1 = list5.SingleOrDefault();
            var single2 = list5.SingleOrDefault(1);
        }

        public void NonGenericMethod(int[] list6)
        {
            var single1 = list6.SingleOrDefault();
            var single2 = list6.SingleOrDefault(1);
        }

        public void NonGenericMethod(int[][] list7, int[] fallback)
        {
            var single1 = list7.SingleOrDefault();
            var single2 = list7.SingleOrDefault(fallback);
        }

        public void GenericMethod<T>(CustomList<T> list8, T fallback)
        {
            var single1 = list8.SingleOrDefault();
            var single2 = list8.SingleOrDefault(fallback);
        }

        public void NonGenericMethod(IReadOnlyList<int> list9)
        {
            var single1 = list9.SingleOrDefault();
            var single2 = list9.SingleOrDefault(1);
        }

        public void NonGenericMethod(string text)
        {
            var single1 = text.SingleOrDefault();
            var single2 = text.SingleOrDefault('a');
        }

        public void NonMatch(IList<int> list)
        {
            var single1 = list.SingleOrDefault(x => x > 0);
            var single2 = list.SingleOrDefault(x => x > 0, 1);
        }

        public void NonMatch2(IList<int>? list)
        {
            var single1 = list?.SingleOrDefault();
            var single2 = list?.SingleOrDefault(1);
        }

        public void NonAccessibleIndexers(OrderedDictionary<int, string> orderedDictionary)
        {
            var third = orderedDictionary.SingleOrDefault();
            var secondFromEnd = orderedDictionary.SingleOrDefault();
        }
    }

    public class ConstrainedGenericClass<L> where L : IList<int>
    {
        public void ConstrainedGenericMethod(L list3)
        {
            var third = list3.SingleOrDefault();
            var secondFromEnd = list3.SingleOrDefault();
        }
    }
}