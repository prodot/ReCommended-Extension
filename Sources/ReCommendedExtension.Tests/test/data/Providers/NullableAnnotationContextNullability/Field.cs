using System;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        string nonNullableReference;
        string? nullableReference;

        int nonNullableValue;
        int? nullableValue;

        void NullCheck()
        {
            if (nonNullableReference != null) { }
            if (nullableReference != null) { }
            if (nonNullableValue != null) { }
            if (nullableValue != null) { }
        }

        void AssigningNull()
        {
            nonNullableReference = null;
            nullableReference = null;
            nonNullableValue = default;
            nullableValue = default;
        }

        void AssigningNonNullable()
        {
            nullableReference = nonNullableReference;
            nullableValue = nonNullableValue;
        }

        void AssigningNullable()
        {
            nonNullableReference = nullableReference;
        }

        void Dereferencing()
        {
            Console.WriteLine(nonNullableReference.Length);
            Console.WriteLine(nullableReference.Length);
            Console.WriteLine(nonNullableValue.ToString());
            Console.WriteLine(nullableValue.ToString());
        }
    }

    class Generic<T>
    {
        T nonNullable;
        T? nullable;

        void NullCheck()
        {
            if (nonNullable != null) { }
        }

        void AssigningNull()
        {
            nonNullable = default;
        }

        void Dereferencing()
        {
            Console.WriteLine(nonNullable.ToString());
        }
    }

    class GenericForReference<T> where T : class
    {
        T nonNullableReference;
        T? nullableReference;

        void NullCheck()
        {
            if (nonNullableReference != null) { }
            if (nullableReference != null) { }
        }

        void AssigningNull()
        {
            nonNullableReference = null;
            nullableReference = null;
        }

        void AssigningNonNullable()
        {
            nullableReference = nonNullableReference;
        }

        void AssigningNullable()
        {
            nonNullableReference = nullableReference;
        }

        void Dereferencing()
        {
            Console.WriteLine(nonNullableReference.ToString());
            Console.WriteLine(nullableReference.ToString());
        }
    }

    class GenericForReferenceNullable<T> where T : class?
    {
        T nonNullableReference;
        T? nullableReference;

        void NullCheck()
        {
            if (nonNullableReference != null) { }
        }

        void AssigningNull()
        {
            nonNullableReference = null;
        }

        void Dereferencing()
        {
            Console.WriteLine(nonNullableReference.ToString());
        }
    }

    class GenericForValue<T> where T : struct
    {
        T nonNullableValue;
        T? nullableValue;

        void NullCheck()
        {
            if (nullableValue != null) { }
        }

        void AssigningNull()
        {
            nonNullableValue = default;
            nullableValue = default;
        }

        void AssigningNonNullable()
        {
            nullableValue = nonNullableValue;
        }

        void Dereferencing()
        {
            Console.WriteLine(nonNullableValue.ToString());
            Console.WriteLine(nullableValue.ToString());
        }
    }

    class GenericNotNull<T> where T : notnull
    {
        T nonNullable;
        T? nullable;

        void NullCheck()
        {
            if (nonNullable != null) { }
        }

        void AssigningNull()
        {
            nonNullable = default;
        }

        void Dereferencing()
        {
            Console.WriteLine(nonNullable.ToString());
        }
    }
}