using System;
using System.Globalization;

namespace Test
{
    public class TimesOnly
    {
        public void ParseExact(ReadOnlySpan<char> s, string[] formats)
        {
            var result = TimeOnly.ParseExact(s, formats, {caret}null);
        }
    }
}