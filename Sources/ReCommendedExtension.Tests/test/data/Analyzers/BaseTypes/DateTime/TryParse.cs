using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, IFormatProvider provider, out DateTime result)
        {
            var result11 = DateTime.TryParse(s, null, out result);
            var result12 = DateTime.TryParse(s1, null, out result);

            var result21 = DateTime.TryParse(s, provider, DateTimeStyles.None, out result);
            var result22 = DateTime.TryParse(s1, provider, DateTimeStyles.None, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles styles, out DateTime result)
        {
            var result11 = DateTime.TryParse(s, provider, out result);
            var result12 = DateTime.TryParse(s1, provider, out result);

            var result21 = DateTime.TryParse(s, provider, styles, out result);
            var result22 = DateTime.TryParse(s1, provider, styles, out result);
        }
    }
}