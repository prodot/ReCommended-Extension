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
            var first = list.Single();
        }
        
        public void GenericMethod<T>(IList<T> list2)
        {
            var first = list2.Single();
        }
        
        public void ConstrainedGenericMethod<L>(L list3) where L : IList<int>
        {
            var first = list3.Single();
        }
        
        public void Method(object list4)
        {
            var first = ((IList<int>)list4).Single();
        }
        
        public void NonGenericMethod(List<int> list5)
        {
            var first = list5.Single();
        }

        public void NonGenericMethod(int[] list6)
        {
            var first = list6.Single();
        }

        public void NonGenericMethod(int[][] list7)
        {
            var first = list7.Single();
        }

        public void GenericMethod<T>(CustomList<T> list8)
        {
            var first = list8.Single();
        }

        public void NonGenericMethod(IReadOnlyList<int> list9)
        {
            var first = list9.Single();
        }

        public void NonGenericMethod(string text)
        {
            var first = text.Single();
        }

        public void NonMatch(IList<int> list)
        {
            var first = list.Single(x => x > 0);
        }

        public void NonMatch2(IList<int>? list)
        {
            var first = list?.Single();
        }
    }
}