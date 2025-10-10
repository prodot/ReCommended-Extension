using System;

namespace Test
{
    public class Arguments
    {
        public void OtherArgument(string s, string format, IFormatProvider provider)
        {
            var result1 = TimeSpan.ParseExact(s, [format], provider);
            var result2 = TimeSpan.ParseExact(s, (string[])[format], provider);
            var result3 = TimeSpan.ParseExact(s, new[] { format }, provider);
            var result4 = TimeSpan.ParseExact(s, new string[] { format }, provider);
        }
    }
}