using System;

namespace Test
{
    public class Strings
    {
        public void Contains(string text)
        {
            var result1 = text.Contains("a");
            var result2 = text.Contains(value: "a");

            var result3 = text.Contains("a", StringComparison.OrdinalIgnoreCase);
            var result4 = text.Contains(value: "a", StringComparison.OrdinalIgnoreCase);
        }

        public void IndexOf(string text)
        {
            var result1 = text.IndexOf("a");
            var result2 = text.IndexOf(value: "a");

            var result3 = text.IndexOf("a", StringComparison.OrdinalIgnoreCase);
            var result4 = text.IndexOf(value: "a", StringComparison.OrdinalIgnoreCase);
        }
    }
}