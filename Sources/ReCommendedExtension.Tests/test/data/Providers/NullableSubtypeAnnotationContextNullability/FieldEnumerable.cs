using System;
using System.Collections.Generic;
using System.Linq;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        IEnumerable<string> nonNullableReferences;
        IEnumerable<string?> nullableReferences;
        IEnumerable<int> nonNullableValues;
        IEnumerable<int?> nullableValues;

        void NullCheck_Iteration()
        {
            foreach (var item in nonNullableReferences)
            {
                if (item != null) { }
            }
            foreach (var item in nullableReferences)
            {
                if (item != null) { }
            }
            foreach (var item in nonNullableValues)
            {
                if (item != null) { }
            }
            foreach (var item in nullableValues)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nonNullableReferences where item != null select item;
            var query1 = from item in nullableReferences where item != null select item;
            var query2 = from item in nonNullableValues where item != null select item;
            var query3 = from item in nullableValues where item != null select item;
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in nonNullableReferences)
            {
                Console.WriteLine(item.Length);
            }
            foreach (var item in nullableReferences)
            {
                Console.WriteLine(item.Length);
            }
            foreach (var item in nonNullableValues)
            {
                Console.WriteLine(item.ToString());
            }
            foreach (var item in nullableValues)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }

    class Generic<T>
    {
        IEnumerable<T> any;
        IEnumerable<T?> invalid;

        void NullCheck_Iteration()
        {
            foreach (var item in any)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in any where item != null select item;
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in any)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }

    class GenericForReference<T> where T : class
    {
        IEnumerable<T> nonNullableReferences;
        IEnumerable<T?> nullableReferences;

        void NullCheck_Iteration()
        {
            foreach (var item in nonNullableReferences)
            {
                if (item != null) { }
            }
            foreach (var item in nullableReferences)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nonNullableReferences where item != null select item;
            var query1 = from item in nullableReferences where item != null select item;
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in nonNullableReferences)
            {
                Console.WriteLine(item.ToString());
            }
            foreach (var item in nullableReferences)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }

    class GenericForReferenceNullable<T> where T : class?
    {
        IEnumerable<T> nullableReferences;
        IEnumerable<T?> invalidReferences;

        void NullCheck_Iteration()
        {
            foreach (var item in nullableReferences)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nullableReferences where item != null select item;
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in nullableReferences)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }

    class GenericForValue<T> where T : struct
    {
        IEnumerable<T> nonNullableValues;
        IEnumerable<T?> nullableValues;

        void NullCheck_Iteration()
        {
            foreach (var item in nullableValues)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query3 = from item in nullableValues where item != null select item;
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in nonNullableValues)
            {
                Console.WriteLine(item.ToString());
            }
            foreach (var item in nullableValues)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }

    class GenericNotNull<T> where T : notnull
    {
        IEnumerable<T> nonNullables;
        IEnumerable<T?> invalid;

        void NullCheck_Iteration()
        {
            foreach (var item in nonNullables)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nonNullables where item != null select item;
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in nonNullables)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}