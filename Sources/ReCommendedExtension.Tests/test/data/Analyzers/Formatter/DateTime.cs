using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantFormatSpecifierProvider(DateTime dateTime, string format, IFormatProvider provider)
        {
            var result11 = dateTime.ToString(null as string);
            var result12 = dateTime.ToString("");

            var result21 = dateTime.ToString(null as IFormatProvider);

            var result31 = dateTime.ToString(null, provider);
            var result32 = dateTime.ToString("", provider);
            var result33 = dateTime.ToString(format, null);
            var result34 = dateTime.ToString("O", provider);
            var result35 = dateTime.ToString("o", provider);
            var result36 = dateTime.ToString("R", provider);
            var result37 = dateTime.ToString("r", provider);
            var result38 = dateTime.ToString("s", provider);
            var result39 = dateTime.ToString("u", provider);
        }

        public void NoDetection(DateTime dateTime, string format, IFormatProvider provider)
        {
            var result1 = dateTime.ToString(format);
            var result2 = dateTime.ToString(provider);
            var result3 = dateTime.ToString(format, provider);
            var result4 = dateTime.ToString("G", provider);
        }
    }
}