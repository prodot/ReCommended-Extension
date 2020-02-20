using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        ValueTask<string> nonNullableReference;
        ValueTask<string?> nullableReference;
        ValueTask<int> nonNullableValue;
        ValueTask<int?> nullableValue;

        async Task NullCheck_Awaiting()
        {
            if (await nonNullableReference != null) { }
            if (await nullableReference != null) { }
            if (await nonNullableValue != null) { }
            if (await nullableValue != null) { }
        }

        void NullCheck_GetAwaiter_GetResult()
        {
            if (nonNullableReference.GetAwaiter().GetResult() != null) { }
            if (nullableReference.GetAwaiter().GetResult() != null) { }
            if (nonNullableValue.GetAwaiter().GetResult() != null) { }
            if (nullableValue.GetAwaiter().GetResult() != null) { }
        }

        void NullCheck_Result()
        {
            if (nonNullableReference.Result != null) { }
            if (nullableReference.Result != null) { }
            if (nonNullableValue.Result != null) { }
            if (nullableValue.Result != null) { }
        }

        async Task Dereferencing_Awaiting()
        {
            Console.WriteLine((await nonNullableReference).Length);
            Console.WriteLine((await nullableReference).Length);
            Console.WriteLine((await nonNullableValue).ToString());
            Console.WriteLine((await nullableValue).ToString());
        }

        void Dereferencing_GetAwaiter_GetResult()
        {
            Console.WriteLine(nonNullableReference.GetAwaiter().GetResult().Length);
            Console.WriteLine(nullableReference.GetAwaiter().GetResult().Length);
            Console.WriteLine(nonNullableValue.GetAwaiter().GetResult().ToString());
            Console.WriteLine(nullableValue.GetAwaiter().GetResult().ToString());
        }

        void Dereferencing_Result()
        {
            Console.WriteLine(nonNullableReference.Result.Length);
            Console.WriteLine(nullableReference.Result.Length);
            Console.WriteLine(nonNullableValue.Result.ToString());
            Console.WriteLine(nullableValue.Result.ToString());
        }
    }

    class Generic<T>
    {
        ValueTask<T> any;
        ValueTask<T?> invalid;

        async Task NullCheck_Awaiting()
        {
            if (await any != null) { }
        }

        void NullCheck_GetAwaiter_GetResult()
        {
            if (any.GetAwaiter().GetResult() != null) { }
        }

        void NullCheck_Result()
        {
            if (any.Result != null) { }
        }

        async Task Dereferencing_Awaiting()
        {
            Console.WriteLine((await any).ToString());
        }

        void Dereferencing_GetAwaiter_GetResult()
        {
            Console.WriteLine(any.GetAwaiter().GetResult().ToString());
        }

        void Dereferencing_Result()
        {
            Console.WriteLine(any.Result.ToString());
        }
    }

    class GenericForReference<T> where T : class
    {
        ValueTask<T> nonNullableReference;
        ValueTask<T?> nullableReference;

        async Task NullCheck_Awaiting()
        {
            if (await nonNullableReference != null) { }
            if (await nullableReference != null) { }
        }

        void NullCheck_GetAwaiter_GetResult()
        {
            if (nonNullableReference.GetAwaiter().GetResult() != null) { }
            if (nullableReference.GetAwaiter().GetResult() != null) { }
        }

        void NullCheck_Result()
        {
            if (nonNullableReference.Result != null) { }
            if (nullableReference.Result != null) { }
        }

        async Task Dereferencing_Awaiting()
        {
            Console.WriteLine((await nonNullableReference).ToString());
            Console.WriteLine((await nullableReference).ToString());
        }

        void Dereferencing_GetAwaiter_GetResult()
        {
            Console.WriteLine(nonNullableReference.GetAwaiter().GetResult().ToString());
            Console.WriteLine(nullableReference.GetAwaiter().GetResult().ToString());
        }

        void Dereferencing_Result()
        {
            Console.WriteLine(nonNullableReference.Result.ToString());
            Console.WriteLine(nullableReference.Result.ToString());
        }
    }

    class GenericForReferenceNullable<T> where T : class?
    {
        ValueTask<T> nullableReference;
        ValueTask<T?> invalidReference;

        async Task NullCheck_Awaiting()
        {
            if (await nullableReference != null) { }
        }

        void NullCheck_GetAwaiter_GetResult()
        {
            if (nullableReference.GetAwaiter().GetResult() != null) { }
        }

        void NullCheck_Result()
        {
            if (nullableReference.Result != null) { }
        }

        async Task Dereferencing_Awaiting()
        {
            Console.WriteLine((await nullableReference).ToString());
        }

        void Dereferencing_GetAwaiter_GetResult()
        {
            Console.WriteLine(nullableReference.GetAwaiter().GetResult().ToString());
        }

        void Dereferencing_Result()
        {
            Console.WriteLine(nullableReference.Result.ToString());
        }
    }

    class GenericForValue<T> where T : struct
    {
        ValueTask<T> nonNullableValue;
        ValueTask<T?> nullableValue;

        async Task NullCheck_Awaiting()
        {
            if (await nullableValue != null) { }
        }

        void NullCheck_GetAwaiter_GetResult()
        {
            if (nullableValue.GetAwaiter().GetResult() != null) { }
        }

        void NullCheck_Result()
        {
            if (nullableValue.Result != null) { }
        }

        async Task Dereferencing_Awaiting()
        {
            Console.WriteLine((await nonNullableValue).ToString());
            Console.WriteLine((await nullableValue).ToString());
        }

        void Dereferencing_GetAwaiter_GetResult()
        {
            Console.WriteLine(nonNullableValue.GetAwaiter().GetResult().ToString());
            Console.WriteLine(nullableValue.GetAwaiter().GetResult().ToString());
        }

        void Dereferencing_Result()
        {
            Console.WriteLine(nonNullableValue.Result.ToString());
            Console.WriteLine(nullableValue.Result.ToString());
        }
    }

    class GenericNotNull<T> where T : notnull
    {
        ValueTask<T> nonNullable;
        ValueTask<T?> invalid;

        async Task NullCheck_Awaiting()
        {
            if (await nonNullable != null) { }
        }

        void NullCheck_GetAwaiter_GetResult()
        {
            if (nonNullable.GetAwaiter().GetResult() != null) { }
        }

        void NullCheck_Result()
        {
            if (nonNullable.Result != null) { }
        }

        async Task Dereferencing_Awaiting()
        {
            Console.WriteLine((await nonNullable).ToString());
        }

        void Dereferencing_GetAwaiter_GetResult()
        {
            Console.WriteLine(nonNullable.GetAwaiter().GetResult().ToString());
        }

        void Dereferencing_Result()
        {
            Console.WriteLine(nonNullable.Result.ToString());
        }
    }
}