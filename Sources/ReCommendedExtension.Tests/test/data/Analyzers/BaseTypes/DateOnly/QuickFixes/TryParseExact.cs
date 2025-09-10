using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void TryParseExact(string s, string format, out DateOnly result)
        {
            var result1 = DateOnly.TryParseExact(s, format, null,{caret} DateTimeStyles.None, out result);
        }
    }
}