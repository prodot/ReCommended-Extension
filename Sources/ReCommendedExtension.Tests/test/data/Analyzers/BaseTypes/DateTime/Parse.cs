using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, IFormatProvider provider)
        {
            var result11 = DateTime.Parse(s, null);
            var result12 = DateTime.Parse(s, provider, DateTimeStyles.None);

            var result21 = DateTime.Parse(s1, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles styles)
        {
            var result11 = DateTime.Parse(s, provider);
            var result12 = DateTime.Parse(s, provider, styles);

            var result21 = DateTime.Parse(s1, provider);
        }
    }
}