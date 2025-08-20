using System;

namespace Test
{
    public class DatesOnly
    {
        public void RedundantArgument(DateOnly dateOnly, string format, IFormatProvider provider)
        {
            var result11 = dateOnly.ToString(null as string);
            var result12 = dateOnly.ToString("");
            var result13 = dateOnly.ToString("d");

            var result21 = dateOnly.ToString(null as IFormatProvider);

            var result31 = dateOnly.ToString(format, null);
            var result32 = dateOnly.ToString(null, provider);
            var result33 = dateOnly.ToString("", provider);
            var result34 = dateOnly.ToString("d", provider);
            var result35 = dateOnly.ToString("o", provider);
            var result36 = dateOnly.ToString("O", provider);
            var result37 = dateOnly.ToString("r", provider);
            var result38 = dateOnly.ToString("R", provider);
        }

        public void NoDetection(DateOnly dateOnly, string format, IFormatProvider provider)
        {
            var result1 = dateOnly.ToString(format);
            var result2 = dateOnly.ToString(provider);
            var result3 = dateOnly.ToString(format, provider);
        }
    }
}