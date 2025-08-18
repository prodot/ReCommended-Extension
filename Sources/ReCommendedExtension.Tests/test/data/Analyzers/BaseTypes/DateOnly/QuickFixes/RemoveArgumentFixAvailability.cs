using System;

namespace Test
{
    public class DatesOnly
    {
        public void Parse(string s, ReadOnlySpan<char> s1)
        {
            var result1 = DateOnly.Parse(s, null);
            var result2 = DateOnly.Parse(s1, null);
        }

        public void ParseExact(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = DateOnly.ParseExact(s, format, null);
            var result2 = DateOnly.ParseExact(s, formats, null);
            var result3 = DateOnly.ParseExact(s1, formats, null);
        }
    }
}