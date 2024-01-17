using System;
using System.Collections;
using System.Collections.Generic;

namespace TargetDictionary
{
    class CustomCollection : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }

    class CustomCollection<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }

    class CustomCollectionWithAdd<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        public void Add(T item) { }
    }

    public class NonGenericClass
    {
        Stack<int> field01 = new Stack<int>();
        Stack<int> field02 = new Stack<int>(3);
        Stack<int> field04 = new();
        Stack<int> field05 = new(3);
        Queue<int> field07 = new Queue<int>();
        Queue<int> field08 = new Queue<int>(3);
        Queue<int> field10 = new();
        Queue<int> field11 = new(3);
        CustomCollection field13 = new CustomCollection();
        CustomCollection field14 = new();
        CustomCollection<int> field15 = new CustomCollection<int>();
        CustomCollection<int> field16 = new();
        CustomCollectionWithAdd<int> field17 = new CustomCollectionWithAdd<int>();
        CustomCollectionWithAdd<int> field18 = new();

        void Method(IEnumerable<int> seq)
        {
            Stack<int> var01 = new Stack<int>();
            Stack<int> var02 = new Stack<int>(3);
            Stack<int> var03 = new Stack<int>(seq);
            Stack<int> var04 = new();
            Stack<int> var05 = new(3);
            Stack<int> var06 = new(seq);
            Queue<int> var07 = new Queue<int>();
            Queue<int> var08 = new Queue<int>(3);
            Queue<int> var09 = new Queue<int>(seq);
            Queue<int> var10 = new();
            Queue<int> var11 = new(3);
            Queue<int> var12 = new(seq);
            CustomCollection var13 = new CustomCollection();
            CustomCollection var14 = new();
            CustomCollection<int> var15 = new CustomCollection<int>();
            CustomCollection<int> var16 = new();
            CustomCollectionWithAdd<int> var17 = new CustomCollectionWithAdd<int>();
            CustomCollectionWithAdd<int> var18 = new();

            Consumer1(new Stack<int>());
            Consumer1(new Stack<int>(3));
            Consumer1(new Stack<int>(seq));
            Consumer1(new());
            Consumer1(new(3));
            Consumer1(new(seq));
            Consumer2(new Queue<int>());
            Consumer2(new Queue<int>(3));
            Consumer2(new Queue<int>(seq));
            Consumer2(new());
            Consumer2(new(3));
            Consumer2(new(seq));
            Consumer3(new CustomCollection());
            Consumer3(new());
            Consumer4(new CustomCollection<int>());
            Consumer4(new());
            Consumer5(new CustomCollectionWithAdd<int>());
            Consumer5(new());

            ConsumerGeneric1(new Stack<int>());
            ConsumerGeneric1(new Stack<int>(3));
            ConsumerGeneric1(new Stack<int>(seq));
            ConsumerGeneric2<int>(new Queue<int>());
            ConsumerGeneric2(new Queue<int>(3));
            ConsumerGeneric2(new Queue<int>(seq));
            ConsumerGeneric4(new CustomCollection<int>());
            ConsumerGeneric5(new CustomCollectionWithAdd<int>());
        }

        void Consumer1(Stack<int> items) { }
        void Consumer2(Queue<int> items) { }
        void Consumer3(CustomCollection items) { }
        void Consumer4(CustomCollection<int> items) { }
        void Consumer5(CustomCollectionWithAdd<int> items) { }

        void ConsumerGeneric1<T>(Stack<T> items) { }
        void ConsumerGeneric2<T>(Queue<T> items) { }
        void ConsumerGeneric4<T>(CustomCollection<T> items) { }
        void ConsumerGeneric5<T>(CustomCollectionWithAdd<T> items) { }

        Stack<int> Property01 { get; } = new Stack<int>();
        Stack<int> Property02 { get; } = new Stack<int>(3);
        Stack<int> Property04 { get; } = new();
        Stack<int> Property05 { get; } = new(3);
        Queue<int> Property07 { get; set; } = new Queue<int>();
        Queue<int> Property08 { get; set; } = new Queue<int>(3);
        Queue<int> Property10 { get; set; } = new();
        Queue<int> Property11 { get; set; } = new(3);
        CustomCollection Property13 => new CustomCollection();
        CustomCollection Property14 => new();
        CustomCollection<int> Property15 => new CustomCollection<int>();
        CustomCollection<int> Property16 => new();
        CustomCollectionWithAdd<int> Property17 => new CustomCollectionWithAdd<int>();
        CustomCollectionWithAdd<int> Property18 => new();
    }

    public class GenericClass<T>
    {
        Stack<T> field01 = new Stack<T>();
        Stack<T> field02 = new Stack<T>(3);
        Stack<T> field04 = new();
        Stack<T> field05 = new(3);
        Queue<T> field07 = new Queue<T>();
        Queue<T> field08 = new Queue<T>(3);
        Queue<T> field10 = new();
        Queue<T> field11 = new(3);
        CustomCollection<T> field15 = new CustomCollection<T>();
        CustomCollection<T> field16 = new();
        CustomCollectionWithAdd<T> field17 = new CustomCollectionWithAdd<T>();
        CustomCollectionWithAdd<T> field18 = new();

        void Method(IEnumerable<T> seq)
        {
            Stack<T> var01 = new Stack<T>();
            Stack<T> var02 = new Stack<T>(3);
            Stack<T> var03 = new Stack<T>(seq);
            Stack<T> var04 = new();
            Stack<T> var05 = new(3);
            Stack<T> var06 = new(seq);
            Queue<T> var07 = new Queue<T>();
            Queue<T> var08 = new Queue<T>(3);
            Queue<T> var09 = new Queue<T>(seq);
            Queue<T> var10 = new();
            Queue<T> var11 = new(3);
            Queue<T> var12 = new(seq);
            CustomCollection<T> var15 = new CustomCollection<T>();
            CustomCollection<T> var16 = new();
            CustomCollectionWithAdd<T> var17 = new CustomCollectionWithAdd<T>();
            CustomCollectionWithAdd<T> var18 = new();

            Consumer1(new Stack<T>());
            Consumer1(new Stack<T>(3));
            Consumer1(new Stack<T>(seq));
            Consumer1(new());
            Consumer1(new(3));
            Consumer1(new(seq));
            Consumer2(new Queue<T>());
            Consumer2(new Queue<T>(3));
            Consumer2(new Queue<T>(seq));
            Consumer2(new());
            Consumer2(new(3));
            Consumer2(new(seq));
            Consumer4(new CustomCollection<T>());
            Consumer4(new());
            Consumer5(new CustomCollectionWithAdd<T>());
            Consumer5(new());
        }

        void Consumer1(Stack<T> items) { }
        void Consumer2(Queue<T> items) { }
        void Consumer4(CustomCollection<T> items) { }
        void Consumer5(CustomCollectionWithAdd<T> items) { }

        Stack<T> Property01 { get; } = new Stack<T>();
        Stack<T> Property02 { get; } = new Stack<T>(3);
        Stack<T> Property04 { get; } = new();
        Stack<T> Property05 { get; } = new(3);
        Queue<T> Property07 { get; set; } = new Queue<T>();
        Queue<T> Property08 { get; set; } = new Queue<T>(3);
        Queue<T> Property10 { get; set; } = new();
        Queue<T> Property11 { get; set; } = new(3);
        CustomCollection<T> Property15 => new CustomCollection<T>();
        CustomCollection<T> Property16 => new();
        CustomCollectionWithAdd<T> Property17 => new CustomCollectionWithAdd<T>();
        CustomCollectionWithAdd<T> Property18 => new();
    }
}