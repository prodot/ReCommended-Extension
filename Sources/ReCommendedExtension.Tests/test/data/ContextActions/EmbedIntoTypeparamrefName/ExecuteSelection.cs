using System;
using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        /// <summary>
        /// Wraps a {selstart}T{selend} method.
        /// </summary>
        /// <returns>T value</returns>
        /// <remarks>
        /// T
        /// value
        /// </remarks>
        public T Method<T>(T value) => value;
    }
}