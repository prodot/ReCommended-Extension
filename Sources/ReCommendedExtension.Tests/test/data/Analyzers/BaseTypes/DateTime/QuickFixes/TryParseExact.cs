using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void ParseExact(string s, IFormatProvider provider, DateTimeStyles style, out DateTime result)
        {
            var result_ = DateTime.TryParseExact(s, "o", provider{caret}, style, out result);
        }
    }
}