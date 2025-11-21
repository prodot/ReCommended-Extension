using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Methods
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

        public void RangeIndexer(IList<int> list, List<int> list2, CustomList<int> list3, IReadOnlyList<int> list4, CustomListUniqueItems<int> list5, int[] array, int[][] arrays, object obj, string text, int i, Index index, int fallback)
        {
            var result11 = list.ElementAt(i);            
            var result12 = list.ElementAt(index);            

            var result21 = list.First();
            var result22 = list2.First();
            var result23 = list3.First();
            var result24 = list4.First();
            var result25 = list5.First();
            var result26 = array.First();
            var result27 = arrays.First();
            var result28 = ((IList<int>)obj).First();
            var result29 = text.First();

            var result31 = list.FirstOrDefault();
            var result32 = list.FirstOrDefault(fallback);

            var result41 = list.Last();

            var result51 = list.LastOrDefault();
            var result52 = list.LastOrDefault(fallback);

            var result61 = list.LongCount();
            var result62 = array.LongCount();
            var result63 = text.LongCount();

            var result71 = list.Single();

            var result81 = list.SingleOrDefault();
            var result82 = list.SingleOrDefault(fallback);
        }

        public void RangeIndexer<T>(IList<T> list, List<T> list2, CustomList<T> list3, IReadOnlyList<T> list4, CustomListUniqueItems<T> list5, T[] array, T[][] arrays, object obj, int i, Index index, T fallback)
        {
            var result11 = list.ElementAt(i);
            var result12 = list.ElementAt(index);

            var result21 = list.First();
            var result22 = list2.First();
            var result23 = list3.First();
            var result24 = list4.First();
            var result25 = list5.First();
            var result26 = array.First();
            var result27 = arrays.First();
            var result28 = ((IList<T>)obj).First();

            var result31 = list.FirstOrDefault();
            var result32 = list.FirstOrDefault(fallback);

            var result41 = list.Last();

            var result51 = list.LastOrDefault();
            var result52 = list.LastOrDefault(fallback);

            var result61 = list.LongCount();
            var result62 = array.LongCount();

            var result71 = list.Single();

            var result81 = list.SingleOrDefault();
            var result82 = list.SingleOrDefault(fallback);
        }

        public void RangeIndexer<L>(L list, object obj, int i, Index index, int fallback) where L : IList<int>
        {
            var result11 = list.ElementAt(i);
            var result12 = list.ElementAt(index);

            var result21 = list.First();
            var result22 = ((L)obj).First();

            var result31 = list.FirstOrDefault();
            var result32 = list.FirstOrDefault(fallback);

            var result41 = list.Last();

            var result51 = list.LastOrDefault();
            var result52 = list.LastOrDefault(fallback);

            var result61 = list.LongCount();

            var result71 = list.Single();

            var result81 = list.SingleOrDefault();
            var result82 = list.SingleOrDefault(fallback);
        }

        public void SuspiciousElementAccess(ISet<int> collection, IReadOnlySet<int> collection2, Dictionary<int, string>.KeyCollection collection3, HashSet<int> collection4, int i, Index index, int fallback)
        {
            var result11 = collection.ElementAt(i);
            var result12 = collection.ElementAt(index);
            var result13 = Enumerable.ElementAt(collection, i);
            var result14 = Enumerable.ElementAt(collection, index);

            var result21 = collection.ElementAtOrDefault(i);
            var result22 = collection.ElementAtOrDefault(index);
            var result23 = Enumerable.ElementAtOrDefault(collection, i);
            var result24 = Enumerable.ElementAtOrDefault(collection, index);

            var result31 = collection.First();
            var result32 = collection2.First();
            var result33 = collection3.First();
            var result34 = collection4.First();
            var result35 = Enumerable.First(collection);
            var result36 = Enumerable.First(collection2);
            var result37 = Enumerable.First(collection3);
            var result38 = Enumerable.First(collection4);

            var result41 = collection.FirstOrDefault();
            var result42 = collection.FirstOrDefault(fallback);
            var result43 = Enumerable.FirstOrDefault(collection);
            var result44 = Enumerable.FirstOrDefault(collection, -1);

            var result51 = collection.Last();
            var result52 = Enumerable.Last(collection);

            var result61 = collection.LastOrDefault();
            var result62 = collection.LastOrDefault(fallback);
            var result63 = Enumerable.LastOrDefault(collection);
            var result64 = Enumerable.LastOrDefault(collection, -1);

            collection.ElementAt(i);
            collection.ElementAt(index);

            collection.ElementAtOrDefault(i);
            collection.ElementAtOrDefault(index);

            collection.First();
            collection2.First();
            collection3.First();
            collection4.First();

            collection.FirstOrDefault();
            collection.FirstOrDefault(fallback);

            collection.Last();

            collection.LastOrDefault();
            collection.LastOrDefault(fallback);
        }

        public void SuspiciousElementAccess<T>(ISet<T> collection, IReadOnlySet<T> collection2, Dictionary<T, string>.KeyCollection collection3, HashSet<T> collection4, int i, Index index, T fallback)
        {
            var result11 = collection.ElementAt(i);
            var result12 = collection.ElementAt(index);

            var result21 = collection.ElementAtOrDefault(i);
            var result22 = collection.ElementAtOrDefault(index);

            var result31 = collection.First();
            var result32 = collection2.First();
            var result33 = collection3.First();
            var result34 = collection4.First();

            var result41 = collection.FirstOrDefault();
            var result42 = collection.FirstOrDefault(fallback);

            var result51 = collection.Last();

            var result61 = collection.LastOrDefault();
            var result62 = collection.LastOrDefault(fallback);

            collection.ElementAt(i);
            collection.ElementAt(index);

            collection.ElementAtOrDefault(i);
            collection.ElementAtOrDefault(index);

            collection.First();
            collection2.First();
            collection3.First();
            collection4.First();

            collection.FirstOrDefault();
            collection.FirstOrDefault(fallback);

            collection.Last();

            collection.LastOrDefault();
            collection.LastOrDefault(fallback);
        }

        public void SuspiciousElementAccess<C>(C collection, int i, Index index, int fallback) where C : IReadOnlySet<int>
        {
            var result11 = collection.ElementAt(i);
            var result12 = collection.ElementAt(index);

            var result21 = collection.ElementAtOrDefault(i);
            var result22 = collection.ElementAtOrDefault(index);

            var result31 = collection.First();

            var result41 = collection.FirstOrDefault();
            var result42 = collection.FirstOrDefault(fallback);

            var result51 = collection.Last();

            var result61 = collection.LastOrDefault();
            var result62 = collection.LastOrDefault(fallback);

            collection.ElementAt(i);
            collection.ElementAt(index);

            collection.ElementAtOrDefault(i);
            collection.ElementAtOrDefault(index);

            collection.First();

            collection.FirstOrDefault();
            collection.FirstOrDefault(fallback);

            collection.Last();

            collection.LastOrDefault();
            collection.LastOrDefault(fallback);
        }

        public class A { }
        public class B : A { }
        public class C : A { }

        public void NoDetection(IList<int> list, IList<int>? listNullable, List<int> list2, CustomList<int> list3, IReadOnlyList<int> list4, CustomListUniqueItems<int> list5, IList<B> list6, int[] array, int[][] arrays, object obj, string text, OrderedDictionary<int, string> orderedDictionary, int i, Index index, int fallback)
        {
            var result11 = Enumerable.ElementAt(list, i);
            var result12 = Enumerable.ElementAt(list, index);
            var result13 = orderedDictionary.ElementAt(i);
            var result14 = orderedDictionary.ElementAt(index);
            var result15 = list6.ElementAt<A>(i);
            var result16 = list6.ElementAt<A>(index);

            var result21 = Enumerable.ElementAtOrDefault(list, i);
            var result22 = Enumerable.ElementAtOrDefault(list, index);
            var result23 = orderedDictionary.ElementAtOrDefault(i);
            var result24 = orderedDictionary.ElementAtOrDefault(index);

            var result31 = Enumerable.First(list);
            var result32 = orderedDictionary.First();
            var result33 = list6.First<A>();

            var result41 = Enumerable.FirstOrDefault(list);
            var result42 = Enumerable.FirstOrDefault(list, fallback);
            var result43 = orderedDictionary.FirstOrDefault();
            var result44 = listNullable?.FirstOrDefault();
            var result45 = listNullable?.FirstOrDefault(fallback);
            var result46 = list6.FirstOrDefault<A>(new C());
            var result47 = list.FirstOrDefault(i => i > 0);

            var result51 = Enumerable.Last(list);
            var result52 = orderedDictionary.Last();
            var result53 = list6.Last<A>();

            var result61 = Enumerable.LastOrDefault(list);
            var result62 = Enumerable.LastOrDefault(list, fallback);
            var result63 = orderedDictionary.LastOrDefault();
            var result64 = listNullable?.LastOrDefault();
            var result65 = listNullable?.LastOrDefault(fallback);
            var result66 = list6.LastOrDefault<A>(new C());

            var result71 = Enumerable.LongCount(list);
            var result72 = Enumerable.LongCount(array);
            var result73 = Enumerable.LongCount(text);

            var result81 = Enumerable.Single(list);
            var result82 = listNullable?.Single();
            var result83 = list6.Single<A>();

            var result91 = Enumerable.SingleOrDefault(list);
            var result92 = Enumerable.SingleOrDefault(list, fallback);
            var result94 = listNullable?.SingleOrDefault();
            var result95 = listNullable?.SingleOrDefault(fallback);
            var result96 = list6.SingleOrDefault<A>(new C());

            list.ElementAt(i);
            list.ElementAt(index);

            list.First();
            list2.First();
            list3.First();
            list4.First();
            list5.First();
            array.First();
            arrays.First();
            ((IList<int>)obj).First();
            text.First();

            list.FirstOrDefault();
            list.FirstOrDefault(fallback);

            list.Last();

            list.LastOrDefault();
            list.LastOrDefault(fallback);

            list.LongCount();
            array.LongCount();
            text.LongCount();

            list.Single();

            list.SingleOrDefault();
            list.SingleOrDefault(fallback);
        }
    }
}