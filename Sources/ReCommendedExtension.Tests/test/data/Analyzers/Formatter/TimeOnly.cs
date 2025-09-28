using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantFormatSpecifier(TimeOnly value)
        {
            var result1 = $"{value:t}";

            var result2 = string.Format("{0:t}", value);
        }

        public void RedundantFormatSpecifier(TimeOnly? value)
        {
            var result1 = $"{value:t}";

            var result2 = string.Format("{0:t}", value);
        }

        public void RedundantFormatSpecifierProvider(TimeOnly timeOnly, string format, IFormatProvider provider)
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

        public void NoDetection(TimeOnly value)
        {
            var result11 = $"{value:T}";
            var result12 = $"{value:o}";
            var result13 = $"{value:O}";
            var result14 = $"{value:r}";
            var result15 = $"{value:R}";

            var result21 = string.Format("{0:T}", value);
            var result22 = string.Format("{0:o}", value);
            var result23 = string.Format("{0:O}", value);
            var result24 = string.Format("{0:r}", value);
            var result25 = string.Format("{0:R}", value);
        }

        public void NoDetection(TimeOnly? value)
        {
            var result11 = $"{value:T}";
            var result12 = $"{value:o}";
            var result13 = $"{value:O}";
            var result14 = $"{value:r}";
            var result15 = $"{value:R}";

            var result21 = string.Format("{0:T}", value);
            var result22 = string.Format("{0:o}", value);
            var result23 = string.Format("{0:O}", value);
            var result24 = string.Format("{0:r}", value);
            var result25 = string.Format("{0:R}", value);
        }

        public void NoDetection(TimeOnly timeOnly, string format, IFormatProvider provider)
        {
            var result1 = timeOnly.ToString(format);
            var result2 = timeOnly.ToString(provider);
            var result3 = timeOnly.ToString(format, provider);
        }
    }
}