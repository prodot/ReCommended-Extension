using System;

namespace Test
{
    public class Arguments
    {
        public void OtherArgument(string s, string format, IFormatProvider provider, StringSplitOptions options)
        {
            var result11 = TimeSpan.ParseExact(s, [format], provider);
            var result12 = TimeSpan.ParseExact(s, (string[])[format], provider);
            var result13 = TimeSpan.ParseExact(s, new[] { format }, provider);
            var result14 = TimeSpan.ParseExact(s, new string[] { format }, provider);

            var result21 = s.IndexOf("c");
            var result22 = s.IndexOf("\u200b");

            var result31 = s.LastIndexOf("c", StringComparison.Ordinal);

            var result32 = s.Split([";", ":"], options);
            var result33 = s.Split(new[] { ";", ":" }, options);
        }
    }
}