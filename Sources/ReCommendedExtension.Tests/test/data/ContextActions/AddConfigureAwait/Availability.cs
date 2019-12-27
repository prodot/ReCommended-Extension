using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Test
{
    internal class Availability
    {
        async Task TaskWithoutResult()
        {
            aw{on}ait Task.Run(() => { });
            aw{off}ait Task.Yield();
        }

        async Task TaskWithResult()
        {
            aw{on}ait Task.Run(() => 3);
        }

        async Task ValueTask(ValueTask task)
        {
            aw{on}ait task;
        }

        async Task ValueTaskWithResult(ValueTask<int> task)
        {
            aw{on}ait task;
        }

        async Task AsyncDisposable()
        {
            aw{on}ait using (new MemoryStream()) { }
        }

        async Task AsyncDisposable_Variable_Type()
        {
            aw{on}ait using (Stream m = new MemoryStream()) { }
        }

        async Task AsyncDisposable_Variable()
        {
            aw{on}ait using (var m = new MemoryStream()) { }
        }

        async Task AsyncDisposable_Variable_Multiple()
        {
            aw{off}ait using (Stream m1 = new MemoryStream(), m2 = new MemoryStream()) { }
        }

        async Task AsyncDisposable_UsingVar()
        {
            aw{off}ait using var m1 = new MemoryStream();
        }

        async Task AsyncIterator(IAsyncEnumerable<int> sequence)
        {
            aw{on}ait foreach (var item in sequence) { }
        }
    }
}