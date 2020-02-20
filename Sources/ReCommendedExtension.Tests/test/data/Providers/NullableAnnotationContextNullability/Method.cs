using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        string NonNullableReference() => "one";
        string? NullableReference() => null;
        int NonNullableValue() => 1;
        int? NullableValue() => null;

        async Task AsyncTask()
        {
            await Task.Yield();
        }

        IEnumerable<int> Iterator()
        {
            yield break;
        }

        async IAsyncEnumerable<int> AsyncIterator()
        {
            yield break;
        }

        string NonNullableReference_ReturningNull() => null;
        string? NullableReference_ReturningNull() => null;
        int NonNullableValue_ReturningNull() => default;
        int? NullableValue_ReturningNull() => default;

        void NullCheck()
        {
            if (NonNullableReference() != null) { }
            if (NullableReference() != null) { }
            if (NonNullableValue() != null) { }
            if (NullableValue() != null) { }
            if (AsyncTask() != null) { }
            if (Iterator() != null) { }
            if (AsyncIterator() != null) { }
        }

        void Dereferencing()
        {
            Console.WriteLine(NonNullableReference().Length);
            Console.WriteLine(NullableReference().Length);
            Console.WriteLine(NonNullableValue().ToString());
            Console.WriteLine(NullableValue().ToString());
            Console.WriteLine(AsyncTask().IsCompleted);
            Console.WriteLine(Iterator().ToString());
            Console.WriteLine(AsyncIterator().ToString());
        }
    }
}