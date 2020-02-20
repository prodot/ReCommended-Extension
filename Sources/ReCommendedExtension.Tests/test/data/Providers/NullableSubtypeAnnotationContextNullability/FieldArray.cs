using System;
using System.Linq;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        string[] nonNullableReferences;
        string?[] nullableReferences;
        int[] nonNullableValues;
        int?[] nullableValues;

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

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullableReferences.Length; i++)
            {
                if (nonNullableReferences[i] != null) { }
            }
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                if (nullableReferences[i] != null) { }
            }
            for (var i = 0; i < nonNullableValues.Length; i++)
            {
                if (nonNullableValues[i] != null) { }
            }
            for (var i = 0; i < nullableValues.Length; i++)
            {
                if (nullableValues[i] != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nonNullableReferences where item != null select item;
            var query1 = from item in nullableReferences where item != null select item;
            var query2 = from item in nonNullableValues where item != null select item;
            var query3 = from item in nullableValues where item != null select item;
        }

        void AssigningNull()
        {
            for (var i = 0; i < nonNullableReferences.Length; i++)
            {
                nonNullableReferences[i] = null;
            }
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                nullableReferences[i] = null;
            }
            for (var i = 0; i < nonNullableValues.Length; i++)
            {
                nonNullableValues[i] = default;
            }
            for (var i = 0; i < nullableValues.Length; i++)
            {
                nullableValues[i] = default;
            }
        }

        void AssigningNonNullable()
        {
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                nullableReferences[i] = nonNullableReferences[0];
            }
            for (var i = 0; i < nullableValues.Length; i++)
            {
                nullableValues[i] = nonNullableValues[0];
            }
        }

        void AssigningNullable()
        {
            for (var i = 0; i < nonNullableReferences.Length; i++)
            {
                nonNullableReferences[i] = nullableReferences[0];
            }
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

        void Dereferencing_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullableReferences.Length; i++)
            {
                Console.WriteLine(nonNullableReferences[i].Length);
            }
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                Console.WriteLine(nullableReferences[i].Length);
            }
            for (var i = 0; i < nonNullableValues.Length; i++)
            {
                Console.WriteLine(nonNullableValues[i].ToString());
            }
            for (var i = 0; i < nullableValues.Length; i++)
            {
                Console.WriteLine(nullableValues[i].ToString());
            }
        }
    }

    class Generic<T>
    {
        T[] any;
        T?[] invalid;

        void NullCheck_Iteration()
        {
            foreach (var item in any)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < any.Length; i++)
            {
                if (any[i] != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in any where item != null select item;
        }

        void AssigningNull()
        {
            for (var i = 0; i < any.Length; i++)
            {
                any[i] = default;
            }
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in any)
            {
                Console.WriteLine(item.ToString());
            }
        }

        void Dereferencing_Iteration_Indexed()
        {
            for (var i = 0; i < any.Length; i++)
            {
                Console.WriteLine(any[i].ToString());
            }
        }
    }

    class GenericForReference<T> where T : class
    {
        T[] nonNullableReferences;
        T?[] nullableReferences;

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

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullableReferences.Length; i++)
            {
                if (nonNullableReferences[i] != null) { }
            }
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                if (nullableReferences[i] != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nonNullableReferences where item != null select item;
            var query1 = from item in nullableReferences where item != null select item;
        }

        void AssigningNull()
        {
            for (var i = 0; i < nonNullableReferences.Length; i++)
            {
                nonNullableReferences[i] = null;
            }
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                nullableReferences[i] = null;
            }
        }

        void AssigningNonNullable()
        {
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                nullableReferences[i] = nonNullableReferences[0];
            }
        }

        void AssigningNullable()
        {
            for (var i = 0; i < nonNullableReferences.Length; i++)
            {
                nonNullableReferences[i] = nullableReferences[0];
            }
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

        void Dereferencing_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullableReferences.Length; i++)
            {
                Console.WriteLine(nonNullableReferences[i].ToString());
            }
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                Console.WriteLine(nullableReferences[i].ToString());
            }
        }
    }

    class GenericForReferenceNullable<T> where T : class?
    {
        T[] nullableReferences;
        T?[] invalidReferences;

        void NullCheck_Iteration()
        {
            foreach (var item in nullableReferences)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                if (nullableReferences[i] != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nullableReferences where item != null select item;
        }

        void AssigningNull()
        {
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                nullableReferences[i] = null;
            }
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in nullableReferences)
            {
                Console.WriteLine(item.ToString());
            }
        }

        void Dereferencing_Iteration_Indexed()
        {
            for (var i = 0; i < nullableReferences.Length; i++)
            {
                Console.WriteLine(nullableReferences[i].ToString());
            }
        }
    }

    class GenericForValue<T> where T : struct
    {
        T[] nonNullableValues;
        T?[] nullableValues;

        void NullCheck_Iteration()
        {
            foreach (var item in nullableValues)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nullableValues.Length; i++)
            {
                if (nullableValues[i] != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query3 = from item in nullableValues where item != null select item;
        }

        void AssigningNull()
        {
            for (var i = 0; i < nonNullableValues.Length; i++)
            {
                nonNullableValues[i] = default;
            }
            for (var i = 0; i < nullableValues.Length; i++)
            {
                nullableValues[i] = default;
            }
        }

        void AssigningNonNullable()
        {
            for (var i = 0; i < nullableValues.Length; i++)
            {
                nullableValues[i] = nonNullableValues[0];
            }
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

        void Dereferencing_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullableValues.Length; i++)
            {
                Console.WriteLine(nonNullableValues[i].ToString());
            }
            for (var i = 0; i < nullableValues.Length; i++)
            {
                Console.WriteLine(nullableValues[i].ToString());
            }
        }
    }

    class GenericNotNull<T> where T : notnull
    {
        T[] nonNullables;
        T?[] invalid;

        void NullCheck_Iteration()
        {
            foreach (var item in nonNullables)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullables.Length; i++)
            {
                if (nonNullables[i] != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nonNullables where item != null select item;
        }

        void AssigningNull()
        {
            for (var i = 0; i < nonNullables.Length; i++)
            {
                nonNullables[i] = default;
            }
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in nonNullables)
            {
                Console.WriteLine(item.ToString());
            }
        }

        void Dereferencing_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullables.Length; i++)
            {
                Console.WriteLine(nonNullables[i].ToString());
            }
        }
    }
}