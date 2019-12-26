using System;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        Lazy<string> nonNullableReference;
        Lazy<string?> nullableReference;
        Lazy<int> nonNullableValue;
        Lazy<int?> nullableValue;

        void NullCheck_Value()
        {
            if (nonNullableReference.Value != null) { }
            if (nullableReference.Value != null) { }
            if (nonNullableValue.Value != null) { }
            if (nullableValue.Value != null) { }
        }

        void Dereferencing_Value()
        {
            Console.WriteLine(nonNullableReference.Value.Length);
            Console.WriteLine(nullableReference.Value.Length);
            Console.WriteLine(nonNullableValue.Value.ToString());
            Console.WriteLine(nullableValue.Value.ToString());
        }
    }

    class Generic<T>
    {
        Lazy<T> nonNullable;
        Lazy<T?> nullable;

        void NullCheck_Value()
        {
            if (nonNullable.Value != null) { }
        }

        void Dereferencing_Value()
        {
            Console.WriteLine(nonNullable.Value.ToString());
        }
    }

    class GenericForReference<T> where T : class
    {
        Lazy<T> nonNullableReference;
        Lazy<T?> nullableReference;

        void NullCheck_Value()
        {
            if (nonNullableReference.Value != null) { }
            if (nullableReference.Value != null) { }
        }

        void Dereferencing_Value()
        {
            Console.WriteLine(nonNullableReference.Value.ToString());
            Console.WriteLine(nullableReference.Value.ToString());
        }
    }

    class GenericForReferenceNullable<T> where T : class?
    {
        Lazy<T> nonNullableReference;
        Lazy<T?> nullableReference;

        void NullCheck_Value()
        {
            if (nonNullableReference.Value != null) { }
        }

        void Dereferencing_Value()
        {
            Console.WriteLine(nonNullableReference.Value.ToString());
        }
    }

    class GenericForValue<T> where T : struct
    {
        Lazy<T> nonNullableValue;
        Lazy<T?> nullableValue;

        void NullCheck_Value()
        {
            if (nullableValue.Value != null) { }
        }

        void Dereferencing_Value()
        {
            Console.WriteLine(nonNullableValue.Value.ToString());
            Console.WriteLine(nullableValue.Value.ToString());
        }
    }

    class GenericNotNull<T> where T : notnull
    {
        Lazy<T> nonNullable;
        Lazy<T?> nullable;

        void NullCheck_Value()
        {
            if (nonNullable.Value != null) { }
        }

        void Dereferencing_Value()
        {
            Console.WriteLine(nonNullable.Value.ToString());
        }
    }
}