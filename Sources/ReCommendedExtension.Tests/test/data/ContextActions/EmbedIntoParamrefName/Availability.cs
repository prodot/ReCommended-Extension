using System;
using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        /// <summary>
        /// Processes the {selstart:on}count{selend:on}.
        /// </summary>
        /// <returns>cou{on}nt value</returns>
        /// <remarks>
        /// co{on}unt
        /// value
        /// </remarks>
        public int Method(int count) => count;

        // The above method returns a bo{off}ol value.

        /// <summary>
        /// Processes the {selstart:off}cou
        /// nt{selend:off}.
        /// </summary>
        public int Method2(int count) => count;
    }
}