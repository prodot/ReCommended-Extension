using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        /// <summary>
        /// Wraps external {selstart:on}one.two.three{selend:on} function.
        /// </summary>
        /// <returns>tr{on}ue or false</returns>
        /// <remarks>
        /// tr{on}ue
        /// or false
        /// </remarks>
        public bool Method() => true;

        // The above method returns tr{off}ue or false.

        /// <summary>
        /// Wraps external {selstart:off}one.two.
        /// three{selend:off} function.
        /// </summary>
        public int Method2(int x, int y) => true;
    }
}