using System;
using System.Threading;

namespace Test
{
    public class Methods
    {
        public void UnaryOperator(TimeSpan timeSpan, decimal d, int x)
        {
            var result11 = timeSpan.Negate();

            var result21 = decimal.Negate(d);
            var result22 = decimal.Negate(1ul);
            var result23 = decimal.Negate(1u);
            var result24 = decimal.Negate(1L);
            var result25 = decimal.Negate(1);
            var result26 = decimal.Negate(-1);
            var result27 = decimal.Negate(-0x1);
            var result28 = decimal.Negate(0b1);
            var result29 = decimal.Negate('a');
            var result2A = decimal.Negate((byte)1);
            var result2B = decimal.Negate(x);
        }
    }
}