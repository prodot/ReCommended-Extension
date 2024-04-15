using System;
using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        /// <summary>
        /// Wraps a {selstart:on}Version{selend:on} method.
        /// </summary>
        /// <returns>bo{on}ol value</returns>
        /// <remarks>
        /// bo{on}ol
        /// value
        /// </remarks>
        public bool Method() => true;

        // The above method returns a bo{off}ol value.

        /// <summary>
        /// Wraps a {selstart:off}Ver
        /// sion{selend:off} method.
        /// </summary>
        public int Method2(int x, int y) => true;
    }
}