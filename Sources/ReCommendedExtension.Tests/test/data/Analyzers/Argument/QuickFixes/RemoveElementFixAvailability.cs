using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantCollectionElement(string s, IFormatProvider provider)
        {
            var result1 = TimeSpan.ParseExact(s, ["c", "t", "T", "g", "g", "G"], provider);
            var result2 = TimeSpan.ParseExact(s, (string[])["c", "t", "T", "g", "g", "G"], provider);
            var result3 = TimeSpan.ParseExact(s, new[] { "c", "t", "T", "g", "g", "G" }, provider);
            var result4 = TimeSpan.ParseExact(s, new string[] { "c", "t", "T", "g", "g", "G" }, provider);
        }
    }
}