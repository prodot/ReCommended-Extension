using System;
using System.Globalization;

namespace Test
{
    public class TimesOnly
    {
        public void Add(TimeOnly timeOnly, TimeSpan value)
        {
            var result1 = timeOnly.Add(value, out _);
            var result2 = timeOnly.Add(value, out int _);
        }

        public void AddHours(TimeOnly timeOnly, double value)
        {
            var result1 = timeOnly.AddHours(value, out _);
            var result2 = timeOnly.AddHours(value, out int _);
        }

        public void AddMinutes(TimeOnly timeOnly, double value)
        {
            var result1 = timeOnly.AddMinutes(value, out _);
            var result2 = timeOnly.AddMinutes(value, out int _);
        }

        public void Parse(string s, ReadOnlySpan<char> s1)
        {
            var result1 = TimeOnly.Parse(s, null);
            var result2 = TimeOnly.Parse(s1, null);
        }

        public void ParseExact(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = TimeOnly.ParseExact(s, format, null);
            var result2 = TimeOnly.ParseExact(s, formats, null);
            var result3 = TimeOnly.ParseExact(s1, formats, null);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, IFormatProvider? provider, out TimeOnly result)
        {
            var result11 = TimeOnly.TryParse(s, provider, DateTimeStyles.None, out result);
            var result12 = TimeOnly.TryParse(s, null, out result);

            var result21 = TimeOnly.TryParse(s1, provider, DateTimeStyles.None, out result);
            var result22 = TimeOnly.TryParse(s1, null, out result);
        }
    }
}