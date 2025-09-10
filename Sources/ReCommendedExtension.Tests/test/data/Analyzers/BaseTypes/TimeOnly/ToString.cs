using System;

namespace Test
{
    public class TimesOnly
    {
        public void RedundantArgument(TimeOnly timeOnly, string format, IFormatProvider provider)
        {
            var result11 = timeOnly.ToString(null as string);
            var result12 = timeOnly.ToString("");
            var result13 = timeOnly.ToString("t");

            var result21 = timeOnly.ToString(null as IFormatProvider);

            var result31 = timeOnly.ToString(format, null);
            var result32 = timeOnly.ToString(null, provider);
            var result33 = timeOnly.ToString("", provider);
            var result34 = timeOnly.ToString("t", provider);
            var result35 = timeOnly.ToString("o", provider);
            var result36 = timeOnly.ToString("O", provider);
            var result37 = timeOnly.ToString("r", provider);
            var result38 = timeOnly.ToString("R", provider);
        }

        public void NoDetection(TimeOnly timeOnly, string format, IFormatProvider provider)
        {
            var result1 = timeOnly.ToString(format);
            var result2 = timeOnly.ToString(provider);
            var result3 = timeOnly.ToString(format, provider);
        }
    }
}