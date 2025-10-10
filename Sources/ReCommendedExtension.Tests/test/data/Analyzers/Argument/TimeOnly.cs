using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(TimeOnly timeOnly, TimeSpan timeSpanValue, double doubleValue, string s, ReadOnlySpan<char> s1, string format, string[] formats, IFormatProvider provider, out TimeOnly result)
        {
            var result11 = timeOnly.Add(timeSpanValue, out _);
            var result12 = timeOnly.Add(timeSpanValue, out int _);

            var result21 = timeOnly.AddHours(doubleValue, out _);
            var result22 = timeOnly.AddHours(doubleValue, out int _);

            var result31 = timeOnly.AddMinutes(doubleValue, out _);
            var result32 = timeOnly.AddMinutes(doubleValue, out int _);

            var result41 = TimeOnly.Parse(s, null);
            var result42 = TimeOnly.Parse(s1, null);

            var result51 = TimeOnly.ParseExact(s, formats, null);
            var result52 = TimeOnly.ParseExact(s1, formats, null);

            var result61 = TimeOnly.TryParse(s, null, out result);
            var result62 = TimeOnly.TryParse(s1, null, out result);
            var result63 = TimeOnly.TryParse(s, provider, DateTimeStyles.None, out result);
            var result64 = TimeOnly.TryParse(s1, provider, DateTimeStyles.None, out result);
        }

        public void RedundantArgumentRange(string s, ReadOnlySpan<char> s1, string format, ReadOnlySpan<char> format1, string[] formats, out TimeOnly result)
        {
            var result11 = TimeOnly.Parse(s, null, DateTimeStyles.None);

            var result21 = TimeOnly.ParseExact(s, format, null, DateTimeStyles.None);
            var result22 = TimeOnly.ParseExact(s, formats, null, DateTimeStyles.None);
            var result23 = TimeOnly.ParseExact(s1, formats, null, DateTimeStyles.None);

            var result31 = TimeOnly.TryParseExact(s, format, null, DateTimeStyles.None, out result);
            var result32 = TimeOnly.TryParseExact(s1, format1, null, DateTimeStyles.None, out result);
            var result33 = TimeOnly.TryParseExact(s, formats, null, DateTimeStyles.None, out result);
            var result34 = TimeOnly.TryParseExact(s1, formats, null, DateTimeStyles.None, out result);
        }

        public void RedundantCollectionElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out TimeOnly result)
        {
            var result11 = TimeOnly.ParseExact(s, ["t", "t", "T", "o", "O", "r", "R"]);
            var result12 = TimeOnly.ParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"]);
            var result13 = TimeOnly.ParseExact(s, ["t", "t", "T", "o", "O", "r", "R"], provider, style);
            var result14 = TimeOnly.ParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"], provider, style);

            var result21 = TimeOnly.TryParseExact(s, ["t", "t", "T", "o", "O", "r", "R"], out result);
            var result22 = TimeOnly.TryParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"], out result);
            var result23 = TimeOnly.TryParseExact(s, ["t", "t", "T", "o", "O", "r", "R"], provider, style, out result);
            var result24 = TimeOnly.TryParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"], provider, style, out result);
        }

        public void OtherArgument(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out TimeOnly result)
        {
            var result11 = TimeOnly.ParseExact(s, [format]);
            var result12 = TimeOnly.ParseExact(s, "o", provider, style);
            var result13 = TimeOnly.ParseExact(s, "O", provider, style);
            var result14 = TimeOnly.ParseExact(s, "r", provider, style);
            var result15 = TimeOnly.ParseExact(s, "R", provider, style);
            var result16 = TimeOnly.ParseExact(s, [format], provider, style);
            var result17 = TimeOnly.ParseExact(s, ["o", "R"], provider, style);
            var result18 = TimeOnly.ParseExact(s1, ["o", "R"], provider, style);

            var result21 = TimeOnly.TryParseExact(s, [format], out result);
            var result22 = TimeOnly.TryParseExact(s, "o", provider, style, out result);
            var result23 = TimeOnly.TryParseExact(s, "O", provider, style, out result);
            var result24 = TimeOnly.TryParseExact(s, "r", provider, style, out result);
            var result25 = TimeOnly.TryParseExact(s, "R", provider, style, out result);
            var result26 = TimeOnly.TryParseExact(s, [format], provider, style, out result);
            var result27 = TimeOnly.TryParseExact(s, ["o", "R"], provider, style, out result);
            var result28 = TimeOnly.TryParseExact(s1, ["o", "R"], provider, style, out result);
        }

        public void NoDetection(TimeOnly timeOnly, TimeSpan timeSpanValue, double doubleValue, string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, string format, ReadOnlySpan<char> format1, string[] formats, out int wrappedDays, out TimeOnly result)
        {
            var result11 = timeOnly.Add(timeSpanValue, out wrappedDays);
            var result12 = timeOnly.AddHours(doubleValue, out wrappedDays);
            var result13 = timeOnly.AddMinutes(doubleValue, out wrappedDays);

            int _;

            var result21 = timeOnly.Add(timeSpanValue, out _);
            var result22 = timeOnly.AddHours(doubleValue, out _);
            var result23 = timeOnly.AddMinutes(doubleValue, out _);

            var result31 = TimeOnly.Parse(s, provider, style);
            var result32 = TimeOnly.Parse(s, provider);
            var result33 = TimeOnly.Parse(s1, provider);

            var result41 = TimeOnly.ParseExact(s, format, provider, style);
            var result42 = TimeOnly.ParseExact(s, formats, provider);
            var result43 = TimeOnly.ParseExact(s1, formats, provider);

            var result51 = TimeOnly.TryParse(s, provider, out result);
            var result52 = TimeOnly.TryParse(s1, provider, out result);
            var result53 = TimeOnly.TryParse(s, provider, style, out result);
            var result54 = TimeOnly.TryParse(s1, provider, style, out result);

            var result61 = TimeOnly.Parse(s, style: DateTimeStyles.None, provider: null);

            var result71 = TimeOnly.ParseExact(s, formats, provider, style);
            var result72 = TimeOnly.ParseExact(s1, formats, provider, style);

            var result81 = TimeOnly.TryParseExact(s, format, provider, style, out result);
            var result82 = TimeOnly.TryParseExact(s1, format1, provider, style, out result);
            var result83 = TimeOnly.TryParseExact(s, formats, provider, style, out result);
            var result84 = TimeOnly.TryParseExact(s1, formats, provider, style, out result);

            var result91 = TimeOnly.ParseExact(s, ["t", "T", "o", "r"]);
            var result92 = TimeOnly.ParseExact(s1, ["t", "T", "o", "r"]);
            var result93 = TimeOnly.ParseExact(s, ["t", "T", "o", "r"], provider, style);
            var result94 = TimeOnly.ParseExact(s1, ["t", "T", "o", "r"], provider, style);

            var resultA1 = TimeOnly.TryParseExact(s, ["t", "T", "o", "r"], out result);
            var resultA2 = TimeOnly.TryParseExact(s1, ["t", "T", "o", "r"], out result);
            var resultA3 = TimeOnly.TryParseExact(s, ["t", "T", "o", "r"], provider, style, out result);
            var resultA4 = TimeOnly.TryParseExact(s1, ["t", "T", "o", "r"], provider, style, out result);

            var resultB1 = TimeOnly.ParseExact(s, [format, format]);
            var resultB2 = TimeOnly.ParseExact(s, "o", null, style);
            var resultB3 = TimeOnly.ParseExact(s, "O", null, style);
            var resultB4 = TimeOnly.ParseExact(s, "r", null, style);
            var resultB5 = TimeOnly.ParseExact(s, "R", null, style);
            var resultB6 = TimeOnly.ParseExact(s, [format, format], provider, style);
            var resultB7 = TimeOnly.ParseExact(s, ["o", "R"], null, style);
            var resultB8 = TimeOnly.ParseExact(s, ["o", "R", "t"], provider, style);
            var resultB9 = TimeOnly.ParseExact(s1, ["o", "R"], null, style);
            var resultBA = TimeOnly.ParseExact(s1, ["t", "o", "R"], provider, style);

            var resultC1 = TimeOnly.TryParseExact(s, [format, format], out result);
            var resultC2 = TimeOnly.TryParseExact(s, "o", null, style, out result);
            var resultC3 = TimeOnly.TryParseExact(s, "O", null, style, out result);
            var resultC4 = TimeOnly.TryParseExact(s, "r", null, style, out result);
            var resultC5 = TimeOnly.TryParseExact(s, "R", null, style, out result);
            var resultC6 = TimeOnly.TryParseExact(s, [format, format], provider, style, out result);
            var resultC7 = TimeOnly.TryParseExact(s, ["o", "R"], null, style, out result);
            var resultC8 = TimeOnly.TryParseExact(s, ["o", "R", "t"], provider, style, out result);
            var resultC9 = TimeOnly.TryParseExact(s1, ["o", "R"], null, style, out result);
            var resultCA = TimeOnly.TryParseExact(s1, ["t", "o", "R"], provider, style, out result);
        }

        public void NoDetection(TimeOnly timeOnly, TimeSpan timeSpanValue, double doubleValue)
        {
            var result1 = timeOnly.Add(timeSpanValue, out var _);
            var result2 = timeOnly.AddHours(doubleValue, out var _);
            var result3 = timeOnly.AddMinutes(doubleValue, out var _);
        }
    }
}