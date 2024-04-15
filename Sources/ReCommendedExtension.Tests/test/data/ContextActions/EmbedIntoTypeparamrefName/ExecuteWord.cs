using System;
using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        /// <summary>
        /// Wraps a T method.
        /// </summary>
        /// <returns>T{caret} value</returns>
        /// <remarks>
        /// T
        /// value
        /// </remarks>
        public T Method<T>(T value) => value;
    }
}