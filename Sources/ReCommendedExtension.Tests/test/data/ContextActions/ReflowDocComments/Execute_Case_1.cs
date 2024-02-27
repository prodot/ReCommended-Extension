using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Test;

public static class Execute
{
    /// <exception cref="ArgumentNullException"><paramref name="sequence"/> or <paramref name="func"/> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> has requested a cancellation.</exception>{caret}
    [Pure]
    static async ValueTask<A> AggregateCore<T, A>(this IAsyncEnumerable<T> sequence, A seed, Func<A, T, A> func, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}