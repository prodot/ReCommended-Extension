using System;
using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        /// <summary>
        /// Wraps a {selstart:on}T{selend:on} method.
        /// </summary>
        /// <returns>T{on} value</returns>
        /// <remarks>
        /// {on}T
        /// value
        /// </remarks>
        public T Method<T>(T value) => value;

        // The above method returns a bo{off}ol value.

        /// <summary>
        /// Wraps a {selstart:off}T
        /// {selend:off} method.
        /// </summary>
        public T Method2<T>(T x, T y) => x;
    }
}