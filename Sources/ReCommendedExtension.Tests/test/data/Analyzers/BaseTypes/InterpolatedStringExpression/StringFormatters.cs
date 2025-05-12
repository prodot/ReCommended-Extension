using System;

namespace Test
{
    public class InterpolatedStringItems
    {
        public void FallbackFormatters(int number)
        {
            var result1 = $"{number:G}";

            FormattableString result2 = $"{number:G}";
        }
    }
}