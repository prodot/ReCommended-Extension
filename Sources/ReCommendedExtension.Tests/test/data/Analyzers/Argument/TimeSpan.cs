using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(int days, int hours, int minutes, int seconds, int milliseconds, string s, ReadOnlySpan<char> s1, string format, ReadOnlySpan<char> format1, string[] formats, IFormatProvider provider, out TimeSpan result)
        {
            var result11 = new TimeSpan(0, hours, minutes, seconds);
            var result12 = new TimeSpan(days, hours, minutes, seconds, 0);
            var result13 = new TimeSpan(days, hours, minutes, seconds, milliseconds, 0);

            TimeSpan result21 = new(0, hours, minutes, seconds);
            TimeSpan result22 = new(days, hours, minutes, seconds, 0);
            TimeSpan result23 = new(days, hours, minutes, seconds, milliseconds, 0);

            var result31 = TimeSpan.Parse(s, null);

            var result41 = TimeSpan.ParseExact(s, format, provider, TimeSpanStyles.None);
            var result42 = TimeSpan.ParseExact(s, formats, provider, TimeSpanStyles.None);

            var result51 = TimeSpan.TryParse(s, null, out result);
            var result52 = TimeSpan.TryParse(s1, null, out result);

            var result61 = TimeSpan.TryParseExact(s, format, provider, TimeSpanStyles.None, out result);
            var result62 = TimeSpan.TryParseExact(s1, format1, provider, TimeSpanStyles.None, out result);
            var result63 = TimeSpan.TryParseExact(s, formats, provider, TimeSpanStyles.None, out result);
            var result64 = TimeSpan.TryParseExact(s1, formats, provider, TimeSpanStyles.None, out result);
        }

        public void RedundantCollectionElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, TimeSpanStyles style, out TimeSpan result)
        {
            var result11 = TimeSpan.ParseExact(s, ["c", "t", "T", "g", "g", "G"], provider);
            var result12 = TimeSpan.ParseExact(s, ["c", "t", "T", "g", "g", "G"], provider, style);
            var result13 = TimeSpan.ParseExact(s1, ["c", "t", "T", "g", "g", "G"], provider, style);

            var result21 = TimeSpan.TryParseExact(s, ["c", "t", "T", "g", "g", "G"], provider, out result);
            var result22 = TimeSpan.TryParseExact(s1, ["c", "t", "T", "g", "g", "G"], provider, out result);
            var result23 = TimeSpan.TryParseExact(s, ["c", "t", "T", "g", "g", "G"], provider, style, out result);
            var result24 = TimeSpan.TryParseExact(s1, ["c", "t", "T", "g", "g", "G"], provider, style, out result);
        }

        public void NoDetection(int days, int hours, int minutes, int seconds, int milliseconds, int microseconds, string s, ReadOnlySpan<char> s1, IFormatProvider provider, string format, ReadOnlySpan<char> format1, string[] formats, TimeSpanStyles style, out TimeSpan result)
        {
            var result11 = new TimeSpan(days, hours, minutes, seconds);
            var result12 = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            var result13 = new TimeSpan(days, hours, minutes, seconds, milliseconds, microseconds);

            var result21 = TimeSpan.Parse(s, provider);

            var result31 = TimeSpan.ParseExact(s, format, provider, style);
            var result32 = TimeSpan.ParseExact(s, formats, provider, style);

            var result41 = TimeSpan.TryParse(s, provider, out result);
            var result42 = TimeSpan.TryParse(s1, provider, out result);

            var result51 = TimeSpan.TryParseExact(s, format, provider, style, out result);
            var result52 = TimeSpan.TryParseExact(s1, format1, provider, style, out result);
            var result53 = TimeSpan.TryParseExact(s, formats, provider, style, out result);
            var result54 = TimeSpan.TryParseExact(s1, formats, provider, style, out result);

            var result61 = TimeSpan.ParseExact(s, ["c", "g"], provider);
            var result62 = TimeSpan.ParseExact(s, ["c", "g"], provider, style);
            var result63 = TimeSpan.ParseExact(s1, ["c", "g"], provider, style);

            var result71 = TimeSpan.TryParseExact(s, ["c", "g"], provider, out result);
            var result72 = TimeSpan.TryParseExact(s1, ["c", "g"], provider, out result);
            var result73 = TimeSpan.TryParseExact(s, ["c", "g"], provider, style, out result);
            var result74 = TimeSpan.TryParseExact(s1, ["c", "g"], provider, style, out result);
        }
    }
}