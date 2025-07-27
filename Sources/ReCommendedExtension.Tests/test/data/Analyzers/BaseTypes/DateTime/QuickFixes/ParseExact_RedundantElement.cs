using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void ParseExact(string input, IFormatProvider formatProvider, TimeSpanStyles styles)
        {
            var result = DateTime.ParseExact(s, ["r", {caret}"R"], provider, style);
        }
    }
}