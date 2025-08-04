using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void ParseExact(string s, IFormatProvider provider, TimeSpanStyles style)
        {
            var result = DateTime.ParseExact(s, formats: [for{caret}mat], provider, style);
        }
    }
}