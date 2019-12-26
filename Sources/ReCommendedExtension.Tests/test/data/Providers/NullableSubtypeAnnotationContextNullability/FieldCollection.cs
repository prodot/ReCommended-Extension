using System;
using System.Collections.Generic;
using System.Linq;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        List<string> nonNullableReferences;
        List<string?> nullableReferences;
        List<int> nonNullableValues;
        List<int?> nullableValues;

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
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                if (nonNullableReferences[i] != null) { }
            }
            for (var i = 0; i < nullableReferences.Count; i++)
            {
                if (nullableReferences[i] != null) { }
            }
            for (var i = 0; i < nonNullableValues.Count; i++)
            {
                if (nonNullableValues[i] != null) { }
            }
            for (var i = 0; i < nullableValues.Count; i++)
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
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                nonNullableReferences[i] = null;
            }
            for (var i = 0; i < nullableReferences.Count; i++)
            {
                nullableReferences[i] = null;
            }
            for (var i = 0; i < nonNullableValues.Count; i++)
            {
                nonNullableValues[i] = default;
            }
            for (var i = 0; i < nullableValues.Count; i++)
            {
                nullableValues[i] = default;
            }
        }

        void AssigningNonNullable()
        {
            for (var i = 0; i < nullableReferences.Count; i++)
            {
                nullableReferences[i] = nonNullableReferences[0];
            }
            for (var i = 0; i < nullableValues.Count; i++)
            {
                nullableValues[i] = nonNullableValues[0];
            }
        }

        void AssigningNullable()
        {
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                nonNullableReferences[i] = nullableReferences[0];
            }
        }

        void AddingNull()
        {
            nonNullableReferences.Add(null);
            nullableReferences.Add(null);
            nonNullableValues.Add(default);
            nullableValues.Add(default);
        }

        void AddingNonNullable()
        {
            nullableReferences.Add(nonNullableReferences[0]);
            nullableValues.Add(nonNullableValues[0]);
        }

        void AddingNullable()
        {
            nonNullableReferences.Add(nullableReferences[0]);
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
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                Console.WriteLine(nonNullableReferences[i].Length);
            }
            for (var i = 0; i < nullableReferences.Count; i++)
            {
                Console.WriteLine(nullableReferences[i].Length);
            }
            for (var i = 0; i < nonNullableValues.Count; i++)
            {
                Console.WriteLine(nonNullableValues[i].ToString());
            }
            for (var i = 0; i < nullableValues.Count; i++)
            {
                Console.WriteLine(nullableValues[i].ToString());
            }
        }
    }

    class Generic<T>
    {
        List<T> nonNullables;
        List<T?> nullables;

        void NullCheck_Iteration()
        {
            foreach (var item in nonNullables)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullables.Count; i++)
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
            for (var i = 0; i < nonNullables.Count; i++)
            {
                nonNullables[i] = default;
            }
        }

        void AddingNull()
        {
            nonNullables.Add(default);
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
            for (var i = 0; i < nonNullables.Count; i++)
            {
                Console.WriteLine(nonNullables[i].ToString());
            }
        }
    }

    class GenericForReference<T> where T : class
    {
        List<T> nonNullableReferences;
        List<T?> nullableReferences;

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
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                if (nonNullableReferences[i] != null) { }
            }
            for (var i = 0; i < nullableReferences.Count; i++)
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
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                nonNullableReferences[i] = null;
            }
            for (var i = 0; i < nullableReferences.Count; i++)
            {
                nullableReferences[i] = null;
            }
        }

        void AssigningNonNullable()
        {
            for (var i = 0; i < nullableReferences.Count; i++)
            {
                nullableReferences[i] = nonNullableReferences[0];
            }
        }

        void AssigningNullable()
        {
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                nonNullableReferences[i] = nullableReferences[0];
            }
        }

        void AddingNull()
        {
            nonNullableReferences.Add(null);
            nullableReferences.Add(null);
        }

        void AddingNonNullable()
        {
            nullableReferences.Add(nonNullableReferences[0]);
        }

        void AddingNullable()
        {
            nonNullableReferences.Add(nullableReferences[0]);
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
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                Console.WriteLine(nonNullableReferences[i].ToString());
            }
            for (var i = 0; i < nullableReferences.Count; i++)
            {
                Console.WriteLine(nullableReferences[i].ToString());
            }
        }
    }

    class GenericForReferenceNullable<T> where T : class?
    {
        List<T> nonNullableReferences;
        List<T?> nullableReferences;

        void NullCheck_Iteration()
        {
            foreach (var item in nonNullableReferences)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                if (nonNullableReferences[i] != null) { }
            }
        }

        void NullCheck_Linq()
        {
            var query0 = from item in nonNullableReferences where item != null select item;
        }

        void AssigningNull()
        {
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                nonNullableReferences[i] = null;
            }
        }

        void AddingNull()
        {
            nonNullableReferences.Add(null);
        }

        void Dereferencing_Iteration()
        {
            foreach (var item in nonNullableReferences)
            {
                Console.WriteLine(item.ToString());
            }
        }

        void Dereferencing_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullableReferences.Count; i++)
            {
                Console.WriteLine(nonNullableReferences[i].ToString());
            }
        }
    }

    class GenericForValue<T> where T : struct
    {
        List<T> nonNullableValues;
        List<T?> nullableValues;

        void NullCheck_Iteration()
        {
            foreach (var item in nullableValues)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nullableValues.Count; i++)
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
            for (var i = 0; i < nonNullableValues.Count; i++)
            {
                nonNullableValues[i] = default;
            }
            for (var i = 0; i < nullableValues.Count; i++)
            {
                nullableValues[i] = default;
            }
        }

        void AssigningNonNullable()
        {
            for (var i = 0; i < nullableValues.Count; i++)
            {
                nullableValues[i] = nonNullableValues[0];
            }
        }

        void AddingNull()
        {
            nonNullableValues.Add(default);
            nullableValues.Add(default);
        }

        void AddingNonNullable()
        {
            nullableValues.Add(nonNullableValues[0]);
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
            for (var i = 0; i < nonNullableValues.Count; i++)
            {
                Console.WriteLine(nonNullableValues[i].ToString());
            }
            for (var i = 0; i < nullableValues.Count; i++)
            {
                Console.WriteLine(nullableValues[i].ToString());
            }
        }
    }

    class GenericNotNull<T> where T : notnull
    {
        List<T> nonNullables;
        List<T?> nullables;

        void NullCheck_Iteration()
        {
            foreach (var item in nonNullables)
            {
                if (item != null) { }
            }
        }

        void NullCheck_Iteration_Indexed()
        {
            for (var i = 0; i < nonNullables.Count; i++)
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
            for (var i = 0; i < nonNullables.Count; i++)
            {
                nonNullables[i] = default;
            }
        }

        void AddingNull()
        {
            nonNullables.Add(default);
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
            for (var i = 0; i < nonNullables.Count; i++)
            {
                Console.WriteLine(nonNullables[i].ToString());
            }
        }
    }
}