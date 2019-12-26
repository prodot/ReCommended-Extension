using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        async Task<string> NonNullableReference_AsyncTask()
        {
            await Task.Yield();
            return "one";
        }

        async Task<string?> NullableReference_AsyncTask()
        {
            await Task.Yield();
            return null;
        }

        async Task<int> NonNullableValue_AsyncTask()
        {
            await Task.Yield();
            return 1;
        }

        async Task<int?> NullableValue_AsyncTask()
        {
            await Task.Yield();
            return null;
        }

        async ValueTask<string> NonNullableReference_AsyncValueTask()
        {
            await Task.Yield();
            return "one";
        }

        async ValueTask<string?> NullableReference_AsyncValueTask()
        {
            await Task.Yield();
            return null;
        }

        async ValueTask<int> NonNullableValue_AsyncValueTask()
        {
            await Task.Yield();
            return 1;
        }

        async ValueTask<int?> NullableValue_AsyncValueTask()
        {
            await Task.Yield();
            return null;
        }

        IEnumerable<string> NonNullableReference_Iterator()
        {
            yield return "one";
        }

        IEnumerable<string?> NullableReference_Iterator()
        {
            yield return null;
        }

        IEnumerable<int> NonNullableValue_Iterator()
        {
            yield return 1;
        }

        IEnumerable<int?> NullableValue_Iterator()
        {
            yield return null;
        }

        async Task<string> NonNullableReference_AsyncTask_ReturningNullElement()
        {
            await Task.Yield();
            return null;
        }

        async Task<string?> NullableReference_AsyncTask_ReturningNullElement()
        {
            await Task.Yield();
            return null;
        }

        async Task<int> NonNullableValue_AsyncTask_ReturningNullElement()
        {
            await Task.Yield();
            return default;
        }

        async Task<int?> NullableValue_AsyncTask_ReturningNullElement()
        {
            await Task.Yield();
            return default;
        }

        async ValueTask<string> NonNullableReference_AsyncValueTask_ReturningNullElement()
        {
            await Task.Yield();
            return null;
        }

        async ValueTask<string?> NullableReference_AsyncValueTask_ReturningNullElement()
        {
            await Task.Yield();
            return null;
        }

        async ValueTask<int> NonNullableValue_AsyncValueTask_ReturningNullElement()
        {
            await Task.Yield();
            return default;
        }

        async ValueTask<int?> NullableValue_AsyncValueTask_ReturningNullElement()
        {
            await Task.Yield();
            return default;
        }

        IEnumerable<string> NonNullableReference_Iterator_ReturningNullElement()
        {
            yield return null;
        }

        IEnumerable<string?> NullableReference_Iterator_ReturningNullElement()
        {
            yield return null;
        }

        IEnumerable<int> NonNullableValue_Iterator_ReturningNullElement()
        {
            yield return default;
        }

        IEnumerable<int?> NullableValue_Iterator_ReturningNullElement()
        {
            yield return default;
        }
    }
}