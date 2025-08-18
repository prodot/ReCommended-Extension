using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void ParseExact(ReadOnlySpan<char> s, string[] formats)
        {
            var result = DateOnly.ParseExact(s, formats, {caret}null);
        }
    }
}