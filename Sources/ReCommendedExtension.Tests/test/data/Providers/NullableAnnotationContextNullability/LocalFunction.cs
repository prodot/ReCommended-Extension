using System;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        void PassingNull()
        {
            string NonNullableReference(string value) => value;
            string? NullableReference(string? value) => value;
            int NonNullableValue(int value) => value;
            int? NullableValue(int? value) => value;

            NonNullableReference(null);
            NullableReference(null);
            NonNullableValue(default);
            NullableValue(default);
        }

        void NullCheck()
        {
            string NonNullableReference(string value) => value;
            string? NullableReference(string? value) => value;
            int NonNullableValue(int value) => value;
            int? NullableValue(int? value) => value;

            if (NonNullableReference("one") != null) { }
            if (NullableReference("one") != null) { }
            if (NonNullableValue(1) != null) { }
            if (NullableValue(1) != null) { }
        }

        void Dereferencing()
        {
            string NonNullableReference(string value) => value;
            string? NullableReference(string? value) => value;
            int NonNullableValue(int value) => value;
            int? NullableValue(int? value) => value;

            Console.WriteLine(NonNullableReference("one").Length);
            Console.WriteLine(NullableReference("one").Length);
            Console.WriteLine(NonNullableValue(1).ToString());
            Console.WriteLine(NullableValue(1).ToString());
        }
    }
}