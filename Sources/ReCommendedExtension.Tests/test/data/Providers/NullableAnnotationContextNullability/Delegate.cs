namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        delegate string NonNullableReference(string value);
        delegate string? NullableReference(string? value);
        delegate int NonNullableValue(int value);
        delegate int? NullableValue(int? value);

        void PassingNull(NonNullableReference nonNullableReference, NullableReference nullableReference, NonNullableValue nonNullableValue, NullableValue nullableValue)
        {
            nonNullableReference(null);
            nullableReference(null);
            nonNullableValue(default);
            nullableValue(default);
        }

        void CheckingReturnValue(NonNullableReference nonNullableReference, NullableReference nullableReference, NonNullableValue nonNullableValue, NullableValue nullableValue)
        {
            if (nonNullableReference("one") != null) { }
            if (nullableReference("one") != null) { }
            if (nonNullableValue(1) != null) { }
            if (nullableValue(1) != null) { }
        }

        void Dereferencing(NonNullableReference nonNullableReference, NullableReference nullableReference, NonNullableValue nonNullableValue, NullableValue nullableValue)
        {
            System.Console.WriteLine(nonNullableReference("one").Length);
            System.Console.WriteLine(nullableReference("one").Length);
            System.Console.WriteLine(nonNullableValue(1).ToString());
            System.Console.WriteLine(nullableValue(1).ToString());
        }
    }
}