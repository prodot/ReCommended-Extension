using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantFormatSpecifier(DateOnly value)
        {
            var result1 = $"{value:d}";

            var result2 = string.Format("{0:d}", value);
        }

        public void RedundantFormatSpecifier(DateOnly? value)
        {
            var result1 = $"{value:d}";

            var result2 = string.Format("{0:d}", value);
        }

        public void RedundantFormatSpecifierProvider(DateOnly dateOnly, string format, IFormatProvider provider)
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

        public void NoDetection(DateOnly value)
        {
            var result11 = $"{value:D}";
            var result12 = $"{value:o}";
            var result13 = $"{value:O}";
            var result14 = $"{value:r}";
            var result15 = $"{value:R}";
            var result16 = $"{value:m}";
            var result17 = $"{value:M}";
            var result18 = $"{value:y}";
            var result19 = $"{value:Y}";

            var result21 = string.Format("{0:D}", value);
            var result22 = string.Format("{0:o}", value);
            var result23 = string.Format("{0:O}", value);
            var result24 = string.Format("{0:r}", value);
            var result25 = string.Format("{0:R}", value);
            var result26 = string.Format("{0:m}", value);
            var result27 = string.Format("{0:M}", value);
            var result28 = string.Format("{0:y}", value);
            var result29 = string.Format("{0:Y}", value);
        }

        public void NoDetection(DateOnly? value)
        {
            var result11 = $"{value:D}";
            var result12 = $"{value:o}";
            var result13 = $"{value:O}";
            var result14 = $"{value:r}";
            var result15 = $"{value:R}";
            var result16 = $"{value:m}";
            var result17 = $"{value:M}";
            var result18 = $"{value:y}";
            var result19 = $"{value:Y}";

            var result21 = string.Format("{0:D}", value);
            var result22 = string.Format("{0:o}", value);
            var result23 = string.Format("{0:O}", value);
            var result24 = string.Format("{0:r}", value);
            var result25 = string.Format("{0:R}", value);
            var result26 = string.Format("{0:m}", value);
            var result27 = string.Format("{0:M}", value);
            var result28 = string.Format("{0:y}", value);
            var result29 = string.Format("{0:Y}", value);
        }

        public void NoDetection(DateOnly dateOnly, string format, IFormatProvider provider)
        {
            var result1 = dateOnly.ToString(format);
            var result2 = dateOnly.ToString(provider);
            var result3 = dateOnly.ToString(format, provider);
        }
    }
}