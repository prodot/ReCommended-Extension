using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeSpan offset, Calendar calendar, string s, ReadOnlySpan<char> s1, IFormatProvider provider, string format, out DateTimeOffset result)
        {
            var result11 = new DateTimeOffset(year, month, day, hour, minute, second, 0, offset);
            var result12 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, offset);
            var result13 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, 0, calendar, offset);

            DateTimeOffset result21 = new(year, month, day, hour, minute, second, 0, offset);
            DateTimeOffset result22 = new(year, month, day, hour, minute, second, millisecond, 0, offset);
            DateTimeOffset result23 = new(year, month, day, hour, minute, second, millisecond, 0, calendar, offset);

            var result31 = DateTimeOffset.Parse(s, null);
            var result32 = DateTimeOffset.Parse(s1, null);
            var result33 = DateTimeOffset.Parse(s, provider, DateTimeStyles.None);

            var result41 = DateTimeOffset.TryParse(s, null, out result);
            var result42 = DateTimeOffset.TryParse(s1, null, out result);
            var result43 = DateTimeOffset.TryParse(s, provider, DateTimeStyles.None, out result);
            var result44 = DateTimeOffset.TryParse(s1, provider, DateTimeStyles.None, out result);

            var result51 = DateTimeOffset.ParseExact(s, format, provider, DateTimeStyles.None);
        }

        public void RedundantCollectionElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.ParseExact(s, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], provider, style);
            var result12 = DateTimeOffset.ParseExact(s1, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], provider, style);

            var result21 = DateTimeOffset.TryParseExact(s, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], provider, style, out result);
            var result22 = DateTimeOffset.TryParseExact(s1, ["d", "d", "D", "f", "F", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], provider, style, out result);
        }

        public void OtherArgument(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.ParseExact(s, "o", provider);
            var result12 = DateTimeOffset.ParseExact(s, "O", provider);
            var result13 = DateTimeOffset.ParseExact(s, "r", provider);
            var result14 = DateTimeOffset.ParseExact(s, "R", provider);
            var result15 = DateTimeOffset.ParseExact(s, "s", provider);
            var result16 = DateTimeOffset.ParseExact(s, "u", provider);
            var result17 = DateTimeOffset.ParseExact(s, "o", provider, style);
            var result18 = DateTimeOffset.ParseExact(s, "O", provider, style);
            var result19 = DateTimeOffset.ParseExact(s, "r", provider, style);
            var result1A = DateTimeOffset.ParseExact(s, "R", provider, style);
            var result1B = DateTimeOffset.ParseExact(s, "s", provider, style);
            var result1C = DateTimeOffset.ParseExact(s, "u", provider, style);
            var result1D = DateTimeOffset.ParseExact(s, [format], provider, style);
            var result1E = DateTimeOffset.ParseExact(s, ["o", "r", "s", "u"], provider, style);
            var result1F = DateTimeOffset.ParseExact(s1, ["o", "r", "s", "u"], provider, style);

            var result21 = DateTimeOffset.TryParseExact(s, "o", provider, style, out result);
            var result22 = DateTimeOffset.TryParseExact(s, "O", provider, style, out result);
            var result23 = DateTimeOffset.TryParseExact(s, "r", provider, style, out result);
            var result24 = DateTimeOffset.TryParseExact(s, "R", provider, style, out result);
            var result25 = DateTimeOffset.TryParseExact(s, "s", provider, style, out result);
            var result26 = DateTimeOffset.TryParseExact(s, "u", provider, style, out result);
            var result27 = DateTimeOffset.TryParseExact(s, [format], provider, style, out result);
            var result28 = DateTimeOffset.TryParseExact(s, ["o", "r", "s", "u"], provider, style, out result);
            var result29 = DateTimeOffset.TryParseExact(s1, ["o", "r", "s", "u"], provider, style, out result);
        }

        public void NoDetection(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, TimeSpan offset, Calendar calendar, string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, string format, out DateTimeOffset result)
        {
            var result11 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, offset);
            var result12 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, microsecond, offset);
            var result13 = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, microsecond, calendar, offset);

            var result21 = DateTimeOffset.Parse(s, provider);
            var result22 = DateTimeOffset.Parse(s1, provider);
            var result23 = DateTimeOffset.Parse(s, provider, style);

            var result31 = DateTimeOffset.TryParse(s, provider, out result);
            var result32 = DateTimeOffset.TryParse(s1, provider, out result);
            var result33 = DateTimeOffset.TryParse(s, provider, style, out result);
            var result34 = DateTimeOffset.TryParse(s1, provider, style, out result);

            var result41 = DateTimeOffset.ParseExact(s, format, provider, style);

            var result51 = DateTimeOffset.ParseExact(s, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], provider, style);
            var result52 = DateTimeOffset.ParseExact(s1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], provider, style);

            var result61 = DateTimeOffset.TryParseExact(s, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], provider, style, out result);
            var result62 = DateTimeOffset.TryParseExact(s1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], provider, style, out result);

            var result71 = DateTimeOffset.ParseExact(s, "o", null);
            var result72 = DateTimeOffset.ParseExact(s, "O", null);
            var result73 = DateTimeOffset.ParseExact(s, "r", null);
            var result74 = DateTimeOffset.ParseExact(s, "R", null);
            var result75 = DateTimeOffset.ParseExact(s, "s", null);
            var result76 = DateTimeOffset.ParseExact(s, "u", null);
            var result77 = DateTimeOffset.ParseExact(s, "o", null, style);
            var result78 = DateTimeOffset.ParseExact(s, "O", null, style);
            var result79 = DateTimeOffset.ParseExact(s, "r", null, style);
            var result7A = DateTimeOffset.ParseExact(s, "R", null, style);
            var result7B = DateTimeOffset.ParseExact(s, "s", null, style);
            var result7C = DateTimeOffset.ParseExact(s, "u", null, style);
            var result7D = DateTimeOffset.ParseExact(s, [format, format], provider, style);
            var result7E = DateTimeOffset.ParseExact(s, ["o", "r", "s", "u"], null, style);
            var result7F = DateTimeOffset.ParseExact(s, ["o", "r", "s", "u", "d"], provider, style);
            var result7G = DateTimeOffset.ParseExact(s1, ["o", "r", "s", "u"], null, style);
            var result7H = DateTimeOffset.ParseExact(s1, ["d", "o", "r", "s", "u"], provider, style);

            var result81 = DateTimeOffset.TryParseExact(s, "o", null, style, out result);
            var result82 = DateTimeOffset.TryParseExact(s, "O", null, style, out result);
            var result83 = DateTimeOffset.TryParseExact(s, "r", null, style, out result);
            var result84 = DateTimeOffset.TryParseExact(s, "R", null, style, out result);
            var result85 = DateTimeOffset.TryParseExact(s, "s", null, style, out result);
            var result86 = DateTimeOffset.TryParseExact(s, "u", null, style, out result);
            var result87 = DateTimeOffset.TryParseExact(s, [format, format], provider, style, out result);
            var result88 = DateTimeOffset.TryParseExact(s, ["o", "r", "s", "u"], null, style, out result);
            var result89 = DateTimeOffset.TryParseExact(s, ["o", "r", "s", "u", "d"], provider, style, out result);
            var result8A = DateTimeOffset.TryParseExact(s1, ["o", "r", "s", "u"], null, style, out result);
            var result8B = DateTimeOffset.TryParseExact(s1, ["d", "o", "r", "s", "u"], provider, style, out result);
        }
    }
}