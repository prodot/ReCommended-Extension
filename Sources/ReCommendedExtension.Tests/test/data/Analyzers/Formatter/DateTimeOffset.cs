using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantFormatSpecifierProvider(DateTimeOffset dateTimeOffset, string format, IFormatProvider formatProvider)
        {
            var result11 = dateTimeOffset.ToString(null as string);
            var result12 = dateTimeOffset.ToString("");

            var result21 = dateTimeOffset.ToString(null as IFormatProvider);

            var result31 = dateTimeOffset.ToString(null, formatProvider);
            var result32 = dateTimeOffset.ToString("", formatProvider);
            var result33 = dateTimeOffset.ToString(format, null);
            var result34 = dateTimeOffset.ToString("O", formatProvider);
            var result35 = dateTimeOffset.ToString("o", formatProvider);
            var result36 = dateTimeOffset.ToString("R", formatProvider);
            var result37 = dateTimeOffset.ToString("r", formatProvider);
            var result38 = dateTimeOffset.ToString("s", formatProvider);
            var result39 = dateTimeOffset.ToString("u", formatProvider);
        }

        public void NoDetection(DateTimeOffset dateTimeOffset, string format, IFormatProvider formatProvider)
        {
            var result1 = dateTimeOffset.ToString(format);
            var result2 = dateTimeOffset.ToString(formatProvider);
            var result3 = dateTimeOffset.ToString(format, formatProvider);
            var result4 = dateTimeOffset.ToString("G", formatProvider);
        }
    }
}