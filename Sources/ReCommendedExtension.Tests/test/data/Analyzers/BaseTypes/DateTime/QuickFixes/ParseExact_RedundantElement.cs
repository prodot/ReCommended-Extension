using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void ParseExact(string s, IFormatProvider provider, DateTimeStyles style)
        {
            var result = DateTime.ParseExact(s, ["r", {caret}"R"], provider, style);
        }
    }
}