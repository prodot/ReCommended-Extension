using System;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        void Regular_NullCheck(string nonNullableReference, string? nullableReference, int nonNullableValue, int? nullableValue)
        {
            if (nonNullableReference != null) { }
            if (nullableReference != null) { }
            if (nonNullableValue != null) { }
            if (nullableValue != null) { }
        }

        void InParameters_NullCheck(in string nonNullableReference, in string? nullableReference, in int nonNullableValue, in int? nullableValue)
        {
            if (nonNullableReference != null) { }
            if (nullableReference != null) { }
            if (nonNullableValue != null) { }
            if (nullableValue != null) { }
        }

        void RefParameters_NullCheck(ref string nonNullableReference, ref string? nullableReference, ref int nonNullableValue, ref int? nullableValue)
        {
            if (nonNullableReference != null) { }
            if (nullableReference != null) { }
            if (nonNullableValue != null) { }
            if (nullableValue != null) { }
        }

        void ParamsNonNullable_NullCheck(params int[] nonNullableReference)
        {
            if (nonNullableReference != null) { }
        }

        void ParamsNullable_NullCheck(params int[]? nullableReference)
        {
            if (nullableReference != null) { }
        }

        void Regular_AssigningNull(string nonNullableReference, string? nullableReference, int nonNullableValue, int? nullableValue)
        {
            nonNullableReference = null;
            nullableReference = null;
            nonNullableValue = default;
            nullableValue = default;
        }

        void RefParameters_AssigningNull(ref string nonNullableReference, ref string? nullableReference, ref int nonNullableValue, ref int? nullableValue)
        {
            nonNullableReference = null;
            nullableReference = null;
            nonNullableValue = default;
            nullableValue = default;
        }

        void OutParameters_AssigningNull(out string nonNullableReference, out string? nullableReference, out int nonNullableValue, out int? nullableValue)
        {
            nonNullableReference = null;
            nullableReference = null;
            nonNullableValue = default;
            nullableValue = default;
        }

        void ParamsNonNullable_AssigningNull(params int[] nonNullableReference)
        {
            nonNullableReference = null;
        }

        void ParamsNullable_AssigningNull(params int[]? nullableReference)
        {
            nullableReference = null;
        }

        void Regular_AssigningNonNullable(string nonNullableReference, string? nullableReference, int nonNullableValue, int? nullableValue)
        {
            nullableReference = nonNullableReference;
            nullableValue = nonNullableValue;
        }

        void RefParameters_AssigningNonNullable(ref string nonNullableReference, ref string? nullableReference, ref int nonNullableValue, ref int? nullableValue)
        {
            nullableReference = nonNullableReference;
            nullableValue = nonNullableValue;
        }

        void Regular_AssigningNullable(string nonNullableReference, string? nullableReference, int nonNullableValue, int? nullableValue)
        {
            nonNullableReference = nullableReference;
        }

        void RefParameters_AssigningNullable(ref string nonNullableReference, ref string? nullableReference, ref int nonNullableValue, ref int? nullableValue)
        {
            nonNullableReference = nullableReference;
        }

        void Regular_Dereferencing(string nonNullableReference, string? nullableReference, int nonNullableValue, int? nullableValue)
        {
            Console.WriteLine(nonNullableReference.Length);
            Console.WriteLine(nullableReference.Length);
            Console.WriteLine(nonNullableValue.ToString());
            Console.WriteLine(nullableValue.ToString());
        }

        void InParameters_Dereferencing(in string nonNullableReference, in string? nullableReference, in int nonNullableValue, in int? nullableValue)
        {
            Console.WriteLine(nonNullableReference.Length);
            Console.WriteLine(nullableReference.Length);
            Console.WriteLine(nonNullableValue.ToString());
            Console.WriteLine(nullableValue.ToString());
        }

        void RefParameters_Dereferencing(ref string nonNullableReference, ref string? nullableReference, ref int nonNullableValue, ref int? nullableValue)
        {
            Console.WriteLine(nonNullableReference.Length);
            Console.WriteLine(nullableReference.Length);
            Console.WriteLine(nonNullableValue.ToString());
            Console.WriteLine(nullableValue.ToString());
        }

        void ParamsNonNullable_Dereferencing(params int[] nonNullableReference)
        {
            Console.WriteLine(nonNullableReference.Length);
        }

        void ParamsNullable_Dereferencing(params int[]? nullableReference)
        {
            Console.WriteLine(nullableReference.Length);
        }

        void Method(string nonNullableReference, string? nullableReference, int nonNullableValue, int? nullableValue) { }

        void Method_PassingNull()
        {
            Method(null, null, default, default);
        }

        void Method_PassingNonNullable(string nonNullableReference, int nonNullableValue)
        {
            Method(nonNullableReference, nonNullableReference, nonNullableValue, nonNullableValue);
        }

        void Method_PassingNullable(string? nullableReference, int? nullableValue)
        {
            Method(nullableReference, nullableReference, (int)nullableValue, nullableValue);
        }
    }
}