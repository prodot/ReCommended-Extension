using System;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        string NonNullableReference { get; set; }
        string? NullableReference { get; set; }

        int NonNullableValue { get; set; }
        int? NullableValue { get; set; }

        void NullCheck()
        {
            if (NonNullableReference != null) { }
            if (NullableReference != null) { }
            if (NonNullableValue != null) { }
            if (NullableValue != null) { }
        }

        void AssigningNull()
        {
            NonNullableReference = null;
            NullableReference = null;
            NonNullableValue = default;
            NullableValue = default;
        }

        void AssigningNonNullable()
        {
            NullableReference = NonNullableReference;
            NullableValue = NonNullableValue;
        }

        void AssigningNullable()
        {
            NonNullableReference = NullableReference;
        }

        void Dereferencing()
        {
            Console.WriteLine(NonNullableReference.Length);
            Console.WriteLine(NullableReference.Length);
            Console.WriteLine(NonNullableValue.ToString());
            Console.WriteLine(NullableValue.ToString());
        }
    }

    class Generic<T>
    {
        T Any { get; set; }
        T? Invalid { get; set; }

        void NullCheck()
        {
            if (Any != null) { }
        }

        void AssigningNull()
        {
            Any = default;
        }

        void Dereferencing()
        {
            Console.WriteLine(Any.ToString());
        }
    }

    class GenericForReference<T> where T : class
    {
        T NonNullableReference { get; set; }
        T? NullableReference { get; set; }

        void NullCheck()
        {
            if (NonNullableReference != null) { }
            if (NullableReference != null) { }
        }

        void AssigningNull()
        {
            NonNullableReference = null;
            NullableReference = null;
        }

        void AssigningNonNullable()
        {
            NullableReference = NonNullableReference;
        }

        void AssigningNullable()
        {
            NonNullableReference = NullableReference;
        }

        void Dereferencing()
        {
            Console.WriteLine(NonNullableReference.ToString());
            Console.WriteLine(NullableReference.ToString());
        }
    }

    class GenericForReferenceNullable<T> where T : class?
    {
        T NullableReference { get; set; }
        T? InvalidReference { get; set; }

        void NullCheck()
        {
            if (NullableReference != null) { }
        }

        void AssigningNull()
        {
            NullableReference = null;
        }

        void Dereferencing()
        {
            Console.WriteLine(NullableReference.ToString());
        }
    }

    class GenericForValue<T> where T : struct
    {
        T NonNullableValue { get; set; }
        T? NullableValue { get; set; }

        void NullCheck()
        {
            if (NullableValue != null) { }
        }

        void AssigningNull()
        {
            NonNullableValue = default;
            NullableValue = default;
        }

        void AssigningNonNullable()
        {
            NullableValue = NonNullableValue;
        }

        void Dereferencing()
        {
            Console.WriteLine(NonNullableValue.ToString());
            Console.WriteLine(NullableValue.ToString());
        }
    }

    class GenericNotNull<T> where T : notnull
    {
        T NonNullable { get; set; }
        T? Invalid { get; set; }

        void NullCheck()
        {
            if (NonNullable != null) { }
        }

        void AssigningNull()
        {
            NonNullable = default;
        }

        void Dereferencing()
        {
            Console.WriteLine(NonNullable.ToString());
        }
    }
}