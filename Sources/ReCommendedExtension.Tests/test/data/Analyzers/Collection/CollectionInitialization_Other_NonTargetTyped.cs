using System;
using System.Collections;
using System.Collections.Generic;

namespace NonTargetTyped
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

    public class NonGenericClass
    {
        void Method()
        {
            var var1 = new Stack<int>();
            var var2 = new Queue<int>();

            var var3 = new CustomCollection();
            var var4 = new CustomCollection<int>();
        }
    }

    public class GenericClass<T>
    {
        void Method()
        {
            var var1 = new Stack<T>();
            var var2 = new Queue<T>();

            var var4 = new CustomCollection<T>();
        }
    }
}