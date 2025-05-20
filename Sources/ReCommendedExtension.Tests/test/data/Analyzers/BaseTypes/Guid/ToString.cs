using System;

namespace Test
{
    public class Guids
    {
        public void RedundantArgument(Guid guid, string format, IFormatProvider provider)
        {
            var result11 = guid.ToString(null);
            var result12 = guid.ToString("");
            var result13 = guid.ToString("D");
            var result14 = guid.ToString("d");

            var result21 = guid.ToString(format, provider);
        }

        public void NoDetection(Guid guid)
        {
            var result = guid.ToString("N");
        }
    }
}