using System;

namespace Test
{
    public class Int64s
    {
        public void RedundantArgument(long number, string format, IFormatProvider provider)
        {
            var result1 = number.ToString(null as string);
            var result2 = number.ToString("");
            var result3 = number.ToString(null as IFormatProvider);
            var result4 = number.ToString(null, provider);
            var result5 = number.ToString("", provider);
            var result6 = number.ToString(format, null);
            var result7 = number.ToString("", null);
        }

        public void NoDetection(long number, string format, IFormatProvider provider)
        {
            var result1 = number.ToString(format);
            var result2 = number.ToString("D");
            var result3 = number.ToString(provider);
            var result4 = number.ToString(format, provider);
            var result5 = number.ToString("D", provider);
        }
    }
}