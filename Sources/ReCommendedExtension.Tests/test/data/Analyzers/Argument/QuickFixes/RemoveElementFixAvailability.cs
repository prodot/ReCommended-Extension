using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantCollectionElement(string s, IFormatProvider provider)
        {
            var result11 = TimeSpan.ParseExact(s, ["c", "t", "T", "g", "g", "G"], provider);
            var result12 = TimeSpan.ParseExact(s, (string[])["c", "t", "T", "g", "g", "G"], provider);
            var result13 = TimeSpan.ParseExact(s, new[] { "c", "t", "T", "g", "g", "G" }, provider);
            var result14 = TimeSpan.ParseExact(s, new string[] { "c", "t", "T", "g", "g", "G" }, provider);
        }
    }
}