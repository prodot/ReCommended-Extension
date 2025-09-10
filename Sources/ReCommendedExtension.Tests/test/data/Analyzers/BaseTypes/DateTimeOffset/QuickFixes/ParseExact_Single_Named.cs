using System;
using System.Globalization;

namespace Test
{
    public class DateTimeOffsets
    {
        public void ParseExact(string input, IFormatProvider formatProvider, TimeSpanStyles styles)
        {
            var result = DateTime.ParseExact(input, formats: [for{caret}mat], formatProvider, styles);
        }
    }
}