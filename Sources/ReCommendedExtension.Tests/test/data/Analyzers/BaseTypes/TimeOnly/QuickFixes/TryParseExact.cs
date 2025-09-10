using System;
using System.Globalization;

namespace Test
{
    public class TimesOnly
    {
        public void TryParseExact(string s, string format, out TimeOnly result)
        {
            var result1 = TimeOnly.TryParseExact(s, format, null,{caret} DateTimeStyles.None, out result);
        }
    }
}