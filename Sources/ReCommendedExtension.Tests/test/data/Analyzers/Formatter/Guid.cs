using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantFormatSpecifier(Guid value)
        {
            var result11 = $"{value:D}";
            var result12 = $"{value:d}";

            var result21 = string.Format("{0:D}", value);
            var result22 = string.Format("{0:d}", value);
        }

        public void RedundantFormatSpecifier(Guid? value)
        {
            var result11 = $"{value:D}";
            var result12 = $"{value:d}";

            var result21 = string.Format("{0:D}", value);
            var result22 = string.Format("{0:d}", value);
        }

        public void RedundantFormatSpecifierProvider(Guid guid, string format, IFormatProvider provider)
        {
            var result11 = guid.ToString(null);
            var result12 = guid.ToString("");
            var result13 = guid.ToString("D");
            var result14 = guid.ToString("d");

            var result21 = guid.ToString(format, provider);
        }

        public void NoDetection(Guid value)
        {
            var result1 = $"{value:N}";

            var result2 = string.Format("{0:N}", value);

            var result3 = value.ToString("N");
        }

        public void NoDetection(Guid? value)
        {
            var result1 = $"{value:N}";

            var result2 = string.Format("{0:N}", value);
        }
    }
}