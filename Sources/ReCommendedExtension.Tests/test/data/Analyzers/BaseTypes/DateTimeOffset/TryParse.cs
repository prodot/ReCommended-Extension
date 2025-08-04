using System;
using System.Globalization;

namespace Test
{
    public class DateTimeOffsets
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, IFormatProvider provider, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.TryParse(s, null, out result);
            var result12 = DateTimeOffset.TryParse(s1, null, out result);

            var result21 = DateTimeOffset.TryParse(s, provider, DateTimeStyles.None, out result);
            var result22 = DateTimeOffset.TryParse(s1, provider, DateTimeStyles.None, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles styles, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.TryParse(s, provider, out result);
            var result12 = DateTimeOffset.TryParse(s1, provider, out result);

            var result21 = DateTimeOffset.TryParse(s, provider, styles, out result);
            var result22 = DateTimeOffset.TryParse(s1, provider, styles, out result);
        }
    }
}